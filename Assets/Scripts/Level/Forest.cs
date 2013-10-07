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

public class Forest : Painter
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
				
				//GameObject shadowObject = newTree.transform.FindChild("branch_shadows");
	
				float y = newTree.transform.position.y;
				newTree.transform.position = new Vector3(-1000.0f, y, -1000.0f);
				
				m_idleInstances.Add(newTree);
			}
		}
		else
		{
			Debug.LogWarning("No tree-prefab set. No tree instances will be created");
		}
		
		DimensionBodger.ForceLightOffsets();
	}
	
	public override void Paint(Island island)
	{
		int count = 0;
		foreach(var section in m_sections)
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.DisplayProgressBar("Painting Forest", "Painting Sections...", (float)count / (float)m_sections.Length);
#endif
			section.Paint();
			count++;
		}
	}
	
	public override string GetName()
	{
		return "Forest";
	}
	
	void LateUpdate()
	{
		Vector3 worldCentre = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.0f, 0.5f));
		
		Vector3 localCentre = worldCentre- new Vector3(m_startX, 0.0f, m_startZ);
		
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endZ - m_startZ) / (float)m_sectionsZ;
		
		int currentX = (int)((localCentre.x / sectionWidth) );
		int currentZ = (int)((localCentre.z / sectionHeight) );
		
		if(GetSectionIndex(currentX, currentZ) != m_activeSections[4])
		{
			m_newSections[0] = GetSectionIndex(currentX - 1, currentZ - 1);
			m_newSections[1] = GetSectionIndex(currentX, currentZ - 1);
			m_newSections[2] = GetSectionIndex(currentX + 1, currentZ - 1);
			
			m_newSections[3] = GetSectionIndex(currentX - 1, currentZ);
			m_newSections[4] = GetSectionIndex(currentX, currentZ);
			m_newSections[5] = GetSectionIndex(currentX + 1, currentZ);
			
			m_newSections[6] = GetSectionIndex(currentX - 1, currentZ + 1);
			m_newSections[7] = GetSectionIndex(currentX, currentZ + 1);
			m_newSections[8] = GetSectionIndex(currentX + 1, currentZ + 1);
			
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
		m_sections = new ForestSection[m_sectionsX * m_sectionsZ];
		
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endZ - m_startZ) / (float)m_sectionsZ;
		
		for(int x = 0; x < m_sectionsX; ++x)
		{
			for(int z = 0; z < m_sectionsZ; ++z)
			{
				ForestSection newSection = new ForestSection();
				newSection.Reset();
				newSection.SetTreeRadius(m_treeRadius);
				
				m_sections[x * m_sectionsZ + z] = newSection;
				
				newSection.m_origin 	= new Vector2(m_startX, m_startZ) + new Vector2(x * sectionWidth, z * sectionHeight);
				newSection.m_dimensions = new Vector2(sectionWidth, sectionHeight);
			}
		}
		Debug.Log("Complete");
	}
	
	public bool AddInstance(TreeInstance instance)
	{
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endZ - m_startZ) / (float)m_sectionsZ;
		
		// Determine the target section.
		float x = instance.position.x - m_startX;
		float z = instance.position.z - m_startZ;
		
		x /= sectionWidth;
		z /= sectionHeight;
		
		bool willCollide = WillCollide(instance);
		
		if(!willCollide)
		{
			m_sections[(int)x * m_sectionsZ + (int)z].AddInstance(this, instance);	
		}
		
		return willCollide;
		
	}
	
	public bool WillCollide(TreeInstance instance)
	{
		float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
		float sectionHeight = (m_endZ - m_startZ) / (float)m_sectionsZ;
		
		// Determine the target section.
		float currentX = instance.position.x - m_startX;
		float currentZ = instance.position.z - m_startZ;
		
		currentX /= sectionWidth;
		currentZ /= sectionHeight;
		
		bool collided = false;
		
		for(int x = -1; x < 2 && !collided; ++x)
		{
			for(int z = -1; z < 2 && !collided; ++z)
			{		
				int newX = (int)currentX + x;
				int newZ = (int)currentZ + z;
				
				if(newX < 0 || newZ < 0 || newX >= m_sectionsX || newZ >= m_sectionsZ)
				{
					continue;
				}
				
				collided = m_sections[newX * m_sectionsZ + newZ].WillCollide(instance);
			}
		}
		
		return collided;
	}
	
	// TODO: Remove public fields
	public void SetSectionCounts(int sectionsX, int sectionsZ)
	{
		m_sectionsX = sectionsX;
		m_sectionsZ = sectionsZ;
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
			int targetIndex = m_editorSectionX * m_sectionsZ + m_editorSectionZ;
			m_sections[targetIndex].Draw();	
		
		
			float sectionWidth 	= (m_endX - m_startX) / (float)m_sectionsX;
			float sectionHeight = (m_endZ - m_startZ) / (float)m_sectionsZ;
			
			Gizmos.color = Color.red;
			for(int x = 0; x < m_sectionsX; ++x)
			{
				Gizmos.DrawLine(new Vector3(m_startX + (x * sectionWidth), 1.0f, m_startZ), new Vector3(m_startX + (x * sectionWidth), 1.0f, m_endZ));	
			}
			
			for(int z = 0; z < m_sectionsZ; ++z)
			{
				Gizmos.DrawLine(new Vector3(m_startX, 1.0f, m_startZ + (z * sectionHeight)), new Vector3(m_endX, 1.0f, m_startZ + (z * sectionHeight)));	
			}
		}
	}
	
	private int GetSectionIndex(int x, int z)
	{
		return x * m_sectionsZ + z;	
	}
	
	
	
	public List<int> m_ignoreLayers				= new List<int>();
	public bool m_showLayers					= false;
	public bool m_enabledDebugRendering			= false;
	public float m_treeRadius					= 1.0f;
	public float m_colliderExpansionFactor		= 0.0f;
	public float m_startX						= -100.0f;
	public float m_endX							= 100.0f;
	public float m_startZ						= -100.0f;
	public float m_endZ							= 100.0f;
	public int m_sectionsX						= 30;
	public int m_sectionsZ						= 30;
	public int m_instanceCount					= 200;
	public int m_activeInstanceCount			= 200;
	public int m_editorSectionX					= 0;
	public int m_editorSectionZ					= 0;
	public int m_retryBailout					= 50;
	public GameObject m_treePrefab 				= null;
	public ForestSection[] m_sections			= null;
	
	private int[] m_newSections					= new int[9];
	private int[] m_activeSections				= new int[9];
	private List<GameObject> m_idleInstances 	= new List<GameObject>();
	private List<GameObject> m_activeInstances 	= new List<GameObject>();
}
