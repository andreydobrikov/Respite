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
		
		if(m_treePrefab != null)
		{
			m_idleInstances.Clear();
			m_activeInstances.Clear();
			
			for(int treeCount = 0; treeCount < m_activeInstanceCount; ++treeCount)
			{
				GameObject newTree = GameObject.Instantiate(m_treePrefab) as GameObject;
				newTree.transform.parent = transform;
				
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
		Vector2 extents = Camera.main.ScreenToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
		Vector2 centre = Camera.main.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
		Vector2 diff	= extents - centre;
		
		for(int x = -1; x < 2; ++x)
		{
			for(int y = -1; y < 2; ++y)
			{
				Vector2 newPosition = centre + new Vector2((x * extents.x), y * (extents.y));
				
				
				
				m_newSections[x + 1, y + 1] = (int)newPosition.x;
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
		m_activeInstances.Remove(treeObject);
		m_idleInstances.Add(treeObject);
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
				GameObject newObject = new GameObject("Forest Section (" + x + ", " + y + ")");
				ForestSection newSection = newObject.AddComponent<ForestSection>();
				BoxCollider collider = newObject.AddComponent<BoxCollider>();
				collider.isTrigger = true;
				newObject.layer = LayerMask.NameToLayer("TreeLayer");
				newSection.SetTreeRadius(m_treeRadius);
				
				m_sections[x * m_sectionsY + y] = newSection;
				
				newSection.m_origin 	= new Vector2(m_startX, m_startY) + new Vector2(x * sectionWidth, y * sectionHeight);
				newSection.m_dimensions = new Vector2(sectionWidth, sectionHeight);
				
				collider.size = new Vector3(sectionWidth + (sectionWidth * m_colliderExpansionFactor), sectionHeight + (sectionHeight * m_colliderExpansionFactor) , 10.0f);
				collider.center = new Vector3(sectionWidth / 2.0f, sectionHeight / 2.0f, 0.0f);
				
				newObject.transform.parent = transform;
				newObject.transform.position = newSection.m_origin;
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
		
		return m_sections[(int)x * m_sectionsY + (int)y].AddInstance(instance);
		
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
	public GameObject m_treePrefab 				= null;
	public ForestSection[] m_sections			= null;
	
	private int[,] m_newSections				= new int[3,3];
	private int[,] m_activeSections				= new int[3,3];
	private List<GameObject> m_idleInstances 	= new List<GameObject>();
	private List<GameObject> m_activeInstances 	= new List<GameObject>();
}
