///////////////////////////////////////////////////////////
// 
// PlayerController.cs
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

public class PlayerController : MonoBehaviour 
{
	void Start ()
	{
	//	m_anim = GetComponent<Animator>();
	}

	void Update ()
	{
		Quaternion rotation = transform.localRotation;
		
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		
		Vector2 vals = new Vector2(x, y);
		
		float yRot = rotation.eulerAngles.y;
		Quaternion newRot = Quaternion.Euler(0.0f, 0.0f, yRot);
		
		vals = newRot * new Vector3(x, y, 0.0f);
			
		
		//m_anim.SetFloat("Direction", vals.x);
		//m_anim.SetFloat("Speed", vals.y);
		////m_anim.SetFloat("Sprint", sprintMultiplier);
		
		
		Debug.DrawLine(transform.position, transform.position + (Vector3)vals, Color.red);
		
	}
	
	//private Animator m_anim = null;
}
