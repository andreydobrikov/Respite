/// <summary>
/// Forest section.
/// 
/// Contains the data for a small grid-section of trees and is responsible for requesting
/// activation and deactivation of actual tree instances from the forest.
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ForestSection 
{
	public bool m_debugRenderEnabled 	= false;
	
	[SerializeField]
	public Vector2 m_origin 			= Vector2.one;
	
	[SerializeField]
	public Vector2 m_dimensions 		= Vector2.one;
	
	
	public void SetTreeRadius(float treeRadius)
	{
		m_treeRadius = treeRadius;
	}
	
	public void Start(Forest forest)
	{
		m_forest = forest;
		
		for(int i = 0; i < m_instancePositions.Count; ++i)
		{
			TreeInstance instance = new TreeInstance();
			instance.position = m_instancePositions[i];
			
			m_instances.Add(instance);
		}
	}
	
	public void Reset()
	{
		m_instancePositions.Clear();
	}
	
	public bool AddInstance(Forest forest, TreeInstance instance)
	{
		foreach(var other in m_instancePositions)
		{
			if(Mathf.Abs((instance.position - other).magnitude) < (2.0f * m_treeRadius))
			{
				return false;	
			}
		}
		
		m_instancePositions.Add(instance.position);
		return true;
	}
	
	public void Draw()
	{
		{
			Gizmos.color = new Color(0.0f, 0.0f, 0.5f, 1.0f);
			Gizmos.DrawLine(m_origin, new Vector3(m_origin.x + m_dimensions.x, 1.0f, m_origin.y));
			Gizmos.DrawLine(m_origin, new Vector3(m_origin.x, 1.0f, m_origin.y + m_dimensions.y));
			Gizmos.DrawLine(m_origin + m_dimensions, new Vector3(m_origin.x, 1.0f, m_origin.y + m_dimensions.y));
			Gizmos.DrawLine(m_origin + m_dimensions, new Vector3(m_origin.x + m_dimensions.x, 1.0f, m_origin.y));
			
			Gizmos.color = Color.red;
			foreach(var instance in m_instancePositions)
			{
				Gizmos.DrawSphere(instance + new Vector3(0.0f, 1.0f, 0.0f),  2.0f);	
			}
		}
	}
	
	public void Activate()
	{
		if(m_active)
		{
			return;
		}
		
		m_active = true;
			
		for(int instanceID = 0; instanceID < m_instances.Count; ++instanceID)
		{
			TreeInstance instance = m_instances[instanceID];
			instance.activeObject = m_forest.RequestActivation();
			
			if(instance.activeObject != null)
			{
				float y = instance.activeObject.transform.position.y;
				instance.activeObject.transform.position = (Vector3)instance.position + new Vector3(0.0f, y, 0.0f);
				instance.activeObject.transform.rotation = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);
				
				// TODO: Placeholder shit
				float scale  = UnityEngine.Random.Range(1.0f, 3.5f);
				instance.activeObject.transform.localScale = new Vector3(scale, 1.0f, scale);
			}
		}
	}
	
	public void Deactivate()
	{
		m_active = false;
			
		for(int instanceID = 0; instanceID < m_instances.Count; ++instanceID)
		{
			TreeInstance instance = m_instances[instanceID];
			
			if(instance.activeObject != null)
			{
				float y = instance.activeObject.transform.position.y;
				instance.activeObject.transform.position = new Vector3(-1000.0f, y, -1000.0f);
			}
			
			m_forest.RequestDeactivation(instance.activeObject);
		}
	}
	
	public bool WillCollide(TreeInstance instance)
	{
		foreach(var other in m_instancePositions)
		{
			if(Mathf.Abs((instance.position - other).magnitude) < (2.0f * m_treeRadius))
			{
				return true;	
			}
		}
		
		return false;
	}
	
	[SerializeField]
	private List<Vector3> m_instancePositions	= new List<Vector3>();
	
	private List<TreeInstance> m_instances 		= new List<TreeInstance>();
	private float m_treeRadius 					= 0.0f;
	private Forest m_forest 					= null;
	private bool m_active 						= false;
}

