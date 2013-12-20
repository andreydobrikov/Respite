///////////////////////////////////////////////////////////
// 
// Light.cs
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

[RequireComponent(typeof(MeshRenderer))]
public class Light : ToggleableObject 
{

	void Start ()
	{
		m_renderer = GetComponent<MeshRenderer>();
	}

	public override void ToggleOn() 
	{
		if(m_renderer != null)
		{
			m_renderer.enabled = true;
		}
	}
	
	public override void ToggleOff() 
	{
		if(m_renderer != null)
		{
			m_renderer.enabled = false;
		}
	}
	
	private MeshRenderer m_renderer = null;
}
