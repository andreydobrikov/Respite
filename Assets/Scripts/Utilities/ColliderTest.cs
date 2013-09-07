///////////////////////////////////////////////////////////
// 
// ColliderTest.cs
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

public class ColliderTest : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Lights"))
		{
			collided = true;	
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Lights"))
		{
			collided = false;	
		}
	}
	
	void Start()
	{
		m_collider = GetComponent<BoxCollider>() as BoxCollider;	
	}
	
	void Update()
	{
		if(collided)
		{
			//Debug.DrawLine(m_collider.bounds.min, m_collider.bounds.max, Color.red);
		}
	}
	
	private BoxCollider m_collider = null;
	private bool collided = false;
}
