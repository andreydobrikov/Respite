///////////////////////////////////////////////////////////
// 
// RoofTransition.cs
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

public class RoofTransition : MonoBehaviour 
{
	public float lerpSpeed = 0.05f;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			m_lerpDirection = -lerpSpeed;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			m_lerpDirection = lerpSpeed;
		}
	}
	
	void Update()
	{
		m_lerpProgress += m_lerpDirection;
		
		m_lerpProgress = Mathf.Clamp(m_lerpProgress, 0.0f, 1.0f);
		
		Vector4 color = renderer.material.color;
		
		color.w = m_lerpProgress;
		
		renderer.material.color = color;
	}
	
	private float m_lerpDirection 	= 0.01f;
	private float m_lerpProgress 	= 1.0f;
}
