///////////////////////////////////////////////////////////
// 
// TextureSpin.cs
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

public class TextureSpin : MonoBehaviour 
{
	public float updateSpeed = 0.1f;

	void Start (){

	}

	void Update ()
	{
		m_rotation += updateSpeed;
		
		if(m_rotation > 360.0f)
		{
			m_rotation -= 360.0f;	
		}
		
		Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, m_rotation), Vector3.one);
	
		
        renderer.material.SetMatrix("_Rotation", r);
	}
	
	private float m_rotation = 0.0f;
}
