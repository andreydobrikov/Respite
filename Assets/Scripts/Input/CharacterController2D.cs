using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController2D : MonoBehaviour 
{
	public float MoveSpeed = 200f;
	public float SprintIncrease = 100.0f;
	public float TurnSpeed = 10.0f;
	public float MoveAngle = 50.0f;	// The proximity of the player's direction to their target direction required before they can move. (degrees)
	
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
		// Reset the rigid-body's velocity to avoid inertia
		m_controller.velocity = Vector3.zero;
		
		Vector2 targetDirection = (Vector3.up * (Input.GetAxis("Vertical"))) + (Vector3.right * (Input.GetAxis("Horizontal")));
		float sprintMultiplier = Input.GetAxis("sprint_analogue");
		
		float currentMoveSpeed = MoveSpeed + (Mathf.Sin(sprintMultiplier * Mathf.PI / 2.0f) * SprintIncrease);
		
		if(targetDirection.magnitude > 0.01f)
		{
			targetDirection.Normalize();
			
			Vector3 currentDirection = m_controller.rotation * Vector3.up;
			
			float targetRotation = -Mathf.Atan2(targetDirection.x, targetDirection.y) * (180.0f / Mathf.PI);
			Quaternion newRotation = Quaternion.Euler(0.0f, 0.0f, targetRotation);
			
			targetDirection = newRotation * Vector3.up;
			
			float diffAngle = Quaternion.Angle(newRotation, transform.localRotation);
				
			m_controller.MoveRotation(Quaternion.Slerp(transform.localRotation, newRotation, TurnSpeed / diffAngle));
			
			if(RenderDebugRays)
			{
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.0f), transform.position + ((Vector3)(currentDirection * 3.0f) + new Vector3(0.0f, 0.0f, -1.0f)), new Color(0.5f, 0.0f, 0.0f, 1.0f));
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.0f), transform.position + ((Vector3)(targetDirection * 3.0f) + new Vector3(0.0f, 0.0f, -1.0f)), Color.blue);		
			}
			
			if(diffAngle < MoveAngle)
			{
				m_controller.AddForce( (Vector3.up * (Input.GetAxis("Vertical") * currentMoveSpeed)));
				m_controller.AddForce( (Vector3.right * (Input.GetAxis("Horizontal") * currentMoveSpeed)));
			}
		}
	
	}
}
