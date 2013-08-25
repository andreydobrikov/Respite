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
	PlayerView test = null;
	
	void Start ()
	{
		test = GameObject.FindObjectOfType(typeof(PlayerView)) as PlayerView;
		renderer.enabled = false;
	}

	void LateUpdate ()
	{
		if(Input.GetButtonDown("torch"))
		{
			m_active = !m_active;	
		}
		
		if(m_active)
		{
			Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
		
			if(test != null)
			{
				float angle = Mathf.Atan2(test.Direction.x, test.Direction.y);
				r = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, -angle * Mathf.Rad2Deg), Vector3.one);
			}
			
	        renderer.material.SetMatrix("_Rotation", r);
		}
		
		renderer.enabled = m_active;
		
	}
	
	bool m_active = false;
}
