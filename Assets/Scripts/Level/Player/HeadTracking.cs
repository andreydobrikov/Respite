using UnityEngine;
using System.Collections;

public class HeadTracking : MonoBehaviour 
{
	public GameObject HeadGameObject = null;
	public float LerpRate = 1.0f;

	// Use this for initialization
	void Start () 
	{
		if (HeadGameObject == null)
		{
			Debug.LogError("HeadGameObject not set!");
		}

		m_playerView = GameObject.FindObjectOfType<PlayerView>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_trackingTarget = m_playerView.Direction;

		Vector3 diff = m_trackingTarget;

		Debug.DrawRay(transform.position, m_trackingTarget);

		
		
		float targetAngle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;

		// TODO: Find out whether I have to have animated objects rotated 90 degrees (And so why the rotation is in Z, rather than Y)
		Quaternion targetRotation = Quaternion.Euler(0.0f,targetAngle,   0.0f );
		Quaternion currentRotation = HeadGameObject.transform.rotation;

		Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, LerpRate);

		HeadGameObject.transform.rotation = newRotation;
	}

	private Vector3 m_trackingTarget = Vector3.zero;

	private PlayerView m_playerView = null;
}