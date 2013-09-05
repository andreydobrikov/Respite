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

public class Torch : MonoBehaviour, ISerialisable
{
	PlayerView test = null;
	public bool StartOn = false;
	public bool ReceivesInput = false;
	
	void Start ()
	{
		if(ReceivesInput)
		{
			test = GameObject.FindObjectOfType(typeof(PlayerView)) as PlayerView;
		}
		
		renderer.enabled = false;
		
		m_active = StartOn;
	}

	void LateUpdate ()
	{
		if(ReceivesInput && Input.GetButtonDown("torch"))
		{
			m_active = !m_active;	
		}
		
		if(m_active)
		{
			Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
		
			if(test != null)
			{
				float angle = Mathf.Atan2(test.Direction.x, test.Direction.z);
				r = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, -angle * Mathf.Rad2Deg), Vector3.one);
			}
			else
			{
				Quaternion current = Quaternion.Euler(0.0f, 0.0f, -transform.rotation.eulerAngles.y);
				r = Matrix4x4.TRS(Vector3.zero, current, Vector3.one);	
			}
			
	        renderer.material.SetMatrix("_Rotation", r);
		}
		
		renderer.enabled = m_active;
		
	}
	
	public void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("active", m_active ? "true" : "false"));		
	}
	
	public void SaveDeserialise(List<SavePair> pairs)
	{
		foreach(var pair in pairs)
		{
			if(pair.id == "active")
			{
				m_active = pair.value == "true" ? true : false;	
			}
		}
	}
	
	bool m_active = false;
}
