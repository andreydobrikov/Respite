///////////////////////////////////////////////////////////
// 
// FollowCameraXY.cs
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

public class FollowCameraXY : MonoBehaviour 
{
	private Camera m_camera = null;
	// Use this for initialization
	void Start () 
	{
		m_camera = Camera.mainCamera;
	}
	
	void LateUpdate () 
	{
		transform.position = new Vector3(m_camera.transform.position.x, transform.position.y, m_camera.transform.position.z);
	}
}