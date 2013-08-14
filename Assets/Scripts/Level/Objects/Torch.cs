///////////////////////////////////////////////////////////
// 
// Torch.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Torch : MonoBehaviour 
{
	InteractionMenu test = null;
	
	void Start ()
	{
		test = GameObject.FindObjectOfType(typeof(InteractionMenu)) as InteractionMenu;
		renderer.enabled = false;
	}

	void Update ()
	{
		if(Input.GetButtonDown("torch"))
		{
			m_active = !m_active;	
			
			renderer.enabled = m_active;
		}
		
		if(m_active)
		{
			Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
		
			if(test != null)
			{
				float angle = Mathf.Atan2(test.LastDirection.x, test.LastDirection.y);
				r = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, -angle * Mathf.Rad2Deg), Vector3.one);
			}
			
	        renderer.material.SetMatrix("_Rotation", r);
		}
		
		
	}
	
	bool m_active = false;
}
