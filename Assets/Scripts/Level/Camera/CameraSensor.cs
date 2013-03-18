using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AgentCamera))]
public class CameraSensor : MonoBehaviour 
{
	public LayerMask collisionLayer = 0;
	
	Camera m_camera = null;
	AgentCamera m_agentCamera = null;
	private List<Collider> m_targets = new List<Collider>();
	
	// Use this for initialization
	void Start () 
	{
		m_agentCamera = GetComponent<AgentCamera>();
		m_camera = m_agentCamera.GetCamera();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_camera == null)
		{
			m_camera = m_agentCamera.GetCamera();	
			return;
		}
		
		bool spotted = false;
		foreach(Collider target in m_targets)
		{
			// Create three rays and see if they're inside the cone
			float minAngle = Mathf.Acos(Vector3.Dot(Vector3.up, Vector3.Normalize(target.bounds.min - transform.position)));
			float maxAngle = Mathf.Acos(Vector3.Dot(Vector3.up, Vector3.Normalize(target.bounds.max - transform.position)));
			float centerAngle = Mathf.Acos(Vector3.Dot(Vector3.up, Vector3.Normalize(target.bounds.center - transform.position)));
			
			float cameraMin = m_camera.rotation - m_camera.fov_degrees / 2.0f;
			float cameraMax = m_camera.rotation + m_camera.fov_degrees / 2.0f;
			
			if(minAngle > cameraMin && minAngle < cameraMax)
			{
				Vector3 direction = target.bounds.min - transform.position;
				spotted |= !Physics.Raycast(transform.position, direction, direction.magnitude, collisionLayer);
			}
			
			if(maxAngle > cameraMin && maxAngle < cameraMax)
			{
				Vector3 direction = target.bounds.max - transform.position;
				spotted |= !Physics.Raycast(transform.position, direction, direction.magnitude, collisionLayer);
			}
			
			if(centerAngle > cameraMin && centerAngle < cameraMax)
			{
				Vector3 direction = target.bounds.center- transform.position;
				spotted |= !Physics.Raycast(transform.position, direction, direction.magnitude, collisionLayer);
			}
			
			Debug.DrawLine(transform.position, target.bounds.min, Color.cyan);
			Debug.DrawLine(transform.position, target.bounds.max, Color.cyan);
			Debug.DrawLine(transform.position, target.bounds.center, Color.cyan);
		}
		m_agentCamera.SensorChanged(spotted);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			m_targets.Add(other);	
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		m_targets.Remove(other);
	}
}
