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
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = new Vector3(m_camera.transform.position.x, m_camera.transform.position.y, transform.position.z);
	}
}