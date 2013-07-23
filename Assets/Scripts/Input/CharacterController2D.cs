using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController2D : MonoBehaviour 
{
	public float MoveSpeed = 200f;
	public float TurnSpeed = 0.05f;
	public float MoveAngle = 1.0f;
	
	public bool RenderDebugRays = false;
	
	Rigidbody m_controller = null;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		m_controller.velocity = Vector3.zero;
		Vector2 targetDirection = (Vector3.up * (Input.GetAxis("Vertical"))) + (Vector3.right * (Input.GetAxis("Horizontal")));
		if(targetDirection.magnitude > 0.01f)
		{
		
			targetDirection.Normalize();
			
			
			Vector3 currentDirection = m_controller.rotation * Vector3.up;
			
			
			
			float targetRotation = -Mathf.Atan2(targetDirection.x, targetDirection.y) * (180.0f / Mathf.PI);
			Quaternion newRotation = Quaternion.Euler(0.0f, 0.0f, targetRotation);
			
			targetDirection = newRotation * Vector3.up;
			
			float diffAngle = Quaternion.Angle(newRotation, transform.localRotation);
				
			m_controller.MoveRotation(Quaternion.Slerp(transform.localRotation, newRotation, TurnSpeed / diffAngle));
			//m_controller.rotation = 
			
			if(RenderDebugRays)
			{
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.0f), transform.position + ((Vector3)(currentDirection * 3.0f) + new Vector3(0.0f, 0.0f, -1.0f)), new Color(0.5f, 0.0f, 0.0f, 1.0f));
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.0f), transform.position + ((Vector3)(targetDirection * 3.0f) + new Vector3(0.0f, 0.0f, -1.0f)), Color.blue);		
			}
			
			
			if(diffAngle < MoveAngle)
			{
				m_controller.AddForce( (Vector3.up * (Input.GetAxis("Vertical") * MoveSpeed)));
				m_controller.AddForce( (Vector3.right * (Input.GetAxis("Horizontal") * MoveSpeed)));
			}
		}
	
	}
}
