using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ForestSection : MonoBehaviour 
{
	public bool m_debugRenderEnabled 	= false;
	
	[SerializeField]
	public Vector2 m_origin 			= Vector2.one;
	
	[SerializeField]
	public Vector2 m_dimensions 		= Vector2.one;
	
	[SerializeField]
	public List<TreeInstance> m_instances = new List<TreeInstance>();
	
	public void SetTreeRadius(float treeRadius)
	{
		m_treeRadius = treeRadius;
	}
	
	void Start()
	{
		m_forest = FindObjectOfType(typeof(Forest)) as Forest;	
	}
	
	public bool AddInstance(TreeInstance instance)
	{
		foreach(var other in m_instances)
		{
			if(Mathf.Abs((instance.position - other.position).magnitude) < (2.0f * m_treeRadius))
			{
				return false;	
			}
		}
		
		m_instances.Add(instance);	
		return true;
	}
	
	void OnDrawGizmosSelected()
	{
		if(m_debugRenderEnabled)
		{
			Gizmos.color = new Color(0.0f, 0.0f, 0.5f, 1.0f);
			Gizmos.DrawLine(m_origin, new Vector2(m_origin.x + m_dimensions.x, m_origin.y));
			Gizmos.DrawLine(m_origin, new Vector2(m_origin.x, m_origin.y + m_dimensions.y));
			Gizmos.DrawLine(m_origin + m_dimensions, new Vector2(m_origin.x, m_origin.y + m_dimensions.y));
			Gizmos.DrawLine(m_origin + m_dimensions, new Vector2(m_origin.x + m_dimensions.x, m_origin.y));
			
			Gizmos.color = Color.red;
			foreach(var instance in m_instances)
			{
				Gizmos.DrawSphere((Vector3)instance.position + new Vector3(0.0f, 0.0f, -1.0f),  2.0f);	
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "MainCamera")
		{
			Debug.Log("Activating " + name);	
			
			for(int instanceID = 0; instanceID < m_instances.Count; ++instanceID)
			{
				TreeInstance instance = m_instances[instanceID];
				instance.activeObject = m_forest.RequestActivation();
				
				if(instance.activeObject != null)
				{
					float z = instance.activeObject.transform.position.z;
					instance.activeObject.transform.position = (Vector3)instance.position + new Vector3(0.0f, 0.0f, z);
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "MainCamera")
		{
			Debug.Log("Deactivating " + name);
			
			for(int instanceID = 0; instanceID < m_instances.Count; ++instanceID)
			{
				TreeInstance instance = m_instances[instanceID];
				m_forest.RequestDeactivation(instance.activeObject);
			}
		}
	}
	
	private float m_treeRadius = 0.0f;
	private Forest m_forest = null;
}

