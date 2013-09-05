///////////////////////////////////////////////////////////
// 
// Forest.cs
//
// What it does: Manages a small buffer of tree objects to (hopefully) give the impression of a 
//				 large forest without too many massive GameObjects.
//
// Notes:
// 
// To-do: There's some meaty data-structure optimisation that could go on here, but it shouldn't be hot code.
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Forest : MonoBehaviour
{
	/// <summary>
	/// Start this instance.
	/// Instantiates the pool of available trees from the set prefab.
	/// </summary>
	void Start()
	{	
		if(m_sections == null)
		{
			Debug.LogError("Forest sections not built! No trees will be present");
			RebuildSections();
		}
		
		foreach(var section in m_sections)
		{
			section.Start(this);	
		}
		
		if(m_treePrefab != null)
		{
			m_idleInstances.Clear();
			m_activeInstances.Clear();
			
			for(int treeCount = 0; treeCount < m_activeInstanceCount; ++treeCount)
			{
				GameObject newTree = GameObject.Instantiate(m_treePrefab) as GameObject;
				newTree.transform.parent = transform;
	
				float y = newTree.transform.position.y;
				newTree.transform.position = new Vector3(-1000.0f, y, -1000.0f);
				
				m_idleInstances.Add(newTree);
			}
		}
		else
		{
			Debug.LogWarning("No tree-prefab set. No tree instances will be created");
		}
	}
	
	void LateUpdate()
	{
		Vector2 worldCentre = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));
		Vector2 localCentre = worldCentre- new Vector2(m_startX, m_startY);
		
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endY - m_startY) / (float)m_sectionsY;
		
		int currentX = (int)((localCentre.x / sectionWidth) );
		int currentY = (int)((localCentre.y / sectionHeight) );
		
		if(GetSectionIndex(currentX, currentY) != m_activeSections[4])
		{
			m_newSections[0] = GetSectionIndex(currentX - 1, currentY - 1);
			m_newSections[1] = GetSectionIndex(currentX, currentY - 1);
			m_newSections[2] = GetSectionIndex(currentX + 1, currentY - 1);
			
			m_newSections[3] = GetSectionIndex(currentX - 1, currentY);
			m_newSections[4] = GetSectionIndex(currentX, currentY);
			m_newSections[5] = GetSectionIndex(currentX + 1, currentY);
			
			m_newSections[6] = GetSectionIndex(currentX - 1, currentY + 1);
			m_newSections[7] = GetSectionIndex(currentX, currentY + 1);
			m_newSections[8] = GetSectionIndex(currentX + 1, currentY + 1);
			
			// Deactivate the old sections 
			for(int i = 0; i < 9; ++i)
			{
				bool keepActive = false;
				
				for(int other = 0; other < 9 && !keepActive; ++other)
				{
					if(m_activeSections[i] == m_newSections[other])
					{
						keepActive = true;	
					}
				}
				
				if(!keepActive)
				{
					m_sections[m_activeSections[i]].Deactivate();
				}
			}
			
			// Copy the new values over the old and activate the sections
			for(int i = 0; i < 9; ++i)
			{
				m_activeSections[i] = m_newSections[i];
				m_newSections[i] 	= m_activeSections[i];
				
				m_sections[m_activeSections[i]].Activate();
			}
		}
	}
	
	/// <summary>
	/// Requests the activation of a tree-instance.
	/// </summary>
	/// <returns>
	/// The activated instance if any are free, null otherwise.
	/// </returns>
	public GameObject RequestActivation()
	{
		if(m_idleInstances.Count > 0)
		{
			GameObject activatedObject = m_idleInstances[m_idleInstances.Count - 1];
			
			m_idleInstances.Remove(activatedObject);
			m_activeInstances.Add(activatedObject);
			
			return activatedObject;
		}
		
		Debug.LogWarning("Forest out of trees!");
		return null;
	}
	
	/// <summary>
	/// Rescind control of a tree and deactivate it.
	/// </summary>
	/// <param name='treeObject'>
	/// The tree to deactivate.
	/// </param>
	public void RequestDeactivation(GameObject treeObject)
	{
		if(treeObject != null)
		{
			m_activeInstances.Remove(treeObject);
			m_idleInstances.Add(treeObject);
		}
		
	}
	
	public void RebuildSections()
	{
		Debug.Log("Building sections...");
		m_sections = new ForestSection[m_sectionsX * m_sectionsY];
		
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endY - m_startY) / (float)m_sectionsY;
		
		for(int x = 0; x < m_sectionsX; ++x)
		{
			for(int y = 0; y < m_sectionsY; ++y)
			{
				ForestSection newSection = new ForestSection();
				newSection.Reset();
				newSection.SetTreeRadius(m_treeRadius);
				
				m_sections[x * m_sectionsY + y] = newSection;
				
				newSection.m_origin 	= new Vector2(m_startX, m_startY) + new Vector2(x * sectionWidth, y * sectionHeight);
				newSection.m_dimensions = new Vector2(sectionWidth, sectionHeight);
			}
		}
		Debug.Log("Complete");
	}
	
	public bool AddInstance(TreeInstance instance)
	{
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endY - m_startY) / (float)m_sectionsY;
		
		// Determine the target section.
		float x = instance.position.x - m_startX;
		float y = instance.position.y - m_startY;
		
		x /= sectionWidth;
		y /= sectionHeight;
		
		bool willCollide = WillCollide(instance);
		
		if(!willCollide)
		{
			m_sections[(int)x * m_sectionsY + (int)y].AddInstance(this, instance);	
		}
		
		return willCollide;
		
	}
	
	public bool WillCollide(TreeInstance instance)
	{
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endY - m_startY) / (float)m_sectionsY;
		
		// Determine the target section.
		float currentX = instance.position.x - m_startX;
		float currentY = instance.position.y - m_startY;
		
		currentX /= sectionWidth;
		currentY /= sectionHeight;
		
		bool collided = false;
		
		for(int x = -1; x < 2 && !collided; ++x)
		{
			for(int y = -1; y < 2 && !collided; ++y)
			{		
				int newX = (int)currentX + x;
				int newY = (int)currentY + y;
				
				if(newX < 0 || newY < 0 || newX >= m_sectionsX || newY >= m_sectionsY)
				{
					continue;
				}
				
				collided = m_sections[newX * m_sectionsY + newY].WillCollide(instance);
			}
		}
		
		return collided;
	}
	
	// TODO: Remove public fields
	public void SetSectionCounts(int sectionsX, int sectionsY)
	{
		m_sectionsX = sectionsX;
		m_sectionsY = sectionsY;
	}
	
	public void SetDebugRenderingEnabled(bool debugEnabled)
	{
		m_enabledDebugRendering = debugEnabled;
		foreach(var section in m_sections)
		{
			section.m_debugRenderEnabled = debugEnabled;	
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if(m_enabledDebugRendering)
		{
			int targetIndex = m_editorSectionX * m_sectionsY + m_editorSectionY;
			m_sections[targetIndex].Draw();	
		
		
			float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
			float sectionHeight = (m_endY - m_startY) / (float)m_sectionsY;
			
			Gizmos.color = Color.red;
			for(int x = 0; x < m_sectionsX; ++x)
			{
				Gizmos.DrawLine(new Vector3(m_startX + (x * sectionWidth), m_startY, -1.0f), new Vector3(m_startX + (x * sectionWidth), m_endY, -1.0f));	
			}
			
			for(int y = 0; y < m_sectionsY; ++y)
			{
				Gizmos.DrawLine(new Vector3(m_startX, m_startY + (y * sectionHeight), -1.0f), new Vector3(m_endX, m_startY + (y * sectionHeight), -1.0f));	
			}
		}
	}
	
	private int GetSectionIndex(int x, int y)
	{
		return x * m_sectionsY + y;	
	}
	
	
	
	public List<int> m_ignoreLayers				= new List<int>();
	public bool m_showLayers					= false;
	public bool m_enabledDebugRendering			= false;
	public float m_treeRadius					= 1.0f;
	public float m_colliderExpansionFactor		= 0.0f;
	public float m_startX						= -100.0f;
	public float m_endX							= 100.0f;
	public float m_startY						= -100.0f;
	public float m_endY							= 100.0f;
	public int m_sectionsX						= 30;
	public int m_sectionsY						= 30;
	public int m_instanceCount					= 200;
	public int m_activeInstanceCount			= 200;
	public int m_editorSectionX					= 0;
	public int m_editorSectionY					= 0;
	public int m_retryBailout					= 50;
	public GameObject m_treePrefab 				= null;
	public ForestSection[] m_sections			= null;
	
	private int[] m_newSections					= new int[9];
	private int[] m_activeSections				= new int[9];
	private List<GameObject> m_idleInstances 	= new List<GameObject>();
	private List<GameObject> m_activeInstances 	= new List<GameObject>();
}
