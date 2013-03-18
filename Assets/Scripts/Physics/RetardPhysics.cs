/// <summary>
/// Retard physics.
/// This entire class is sulphurous poison. Abandon all hope, etc...
/// More importantly, get rid of it.
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class RetardPhysics : MonoBehaviour 
{
	public float SlipDiff 			= 3.0f;
	public float MaxSpeed 			= 0.5f;
	
	public int Test { get; set;}
	
	private enum PhysicsState
	{
		Grounded,
		Falling
	}

	private Vector3 m_velocity = new Vector3(0.0f, 0.0f, 0.0f);
	private PhysicsState m_state = PhysicsState.Falling;
	private Collider m_collider = null;
	
	// Use this for initialization
	void Start () 
	{
		m_collider = GetComponent<Collider>();
	}
	
	void FixedUpdate()
	{
		if(Time.deltaTime == 0.0f)
		{
			return;	
		}
		
		switch(m_state)
		{
			case PhysicsState.Falling:
			{
				// Apply gravity
				if(m_velocity.y > -PhysicsSettings.Instance.TerminalVelocity)
				{
					m_velocity += new Vector3(0.0f, PhysicsSettings.Instance.Gravity, 0.0f);	
				}
				
				break;
			}
			
			case PhysicsState.Grounded:
			{
			
				break;
			}
		}
		
		// Apply velocity
		transform.position = transform.position + m_velocity;
		
		// dampen all movement
		if(Mathf.Abs(m_velocity.x) > 0.05f )
		{
			m_velocity.x -= (m_velocity.x / SlipDiff);
		}
		else
		{
			m_velocity.x = 0.0f;	
		}
	}
	
	void OnTriggerEnter(Collider other) 
	{
		if(other.gameObject.tag == "Geometry")
		{
			
			
			
			if(other.bounds.center.y < m_collider.bounds.center.y)
			{
				m_state = PhysicsState.Grounded;
				m_velocity.y = 0.0f;
				
				float y = other.bounds.max.y + m_collider.bounds.extents.y;
				
				transform.position = new Vector3(transform.position.x, y, transform.position.z);
			}
			else
			{
				if(m_collider.bounds.center.y < other.bounds.max.y && m_collider.bounds.center.y > other.bounds.min.y)
				{
					
					if(other.bounds.center.x > m_collider.bounds.max.y && m_velocity.x > 0.0f)
					{
						m_velocity.x = 0.0f;	
					}
				}	
			}
			
			if(other.bounds.min.y > m_collider.bounds.center.y)
			{
					m_velocity.y = 0.0f;
			}
				
			// Find the position of the geometry relative to the physics object, then halt progress in that direction
		}
		
		if(other.gameObject.tag == "Bounds")
		{
			Debug.Log("Bounds hit");
			transform.position = new Vector3(0.0f, 10.0f, 0.0f);	
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Geometry")
		{
			
			if(other.bounds.center.y < m_collider.bounds.center.y)
			{
				m_state = PhysicsState.Grounded;
				
				float y = other.bounds.max.y + m_collider.bounds.extents.y;
				
				transform.position = new Vector3(transform.position.x, y, transform.position.z);
			}
			else 
			{
				if(other.bounds.min.y > m_collider.bounds.max.y)
				{
					if(m_velocity.y > 0.0f)
					{
						m_velocity.y = 0.0f;
					}
				}
				
				if(m_collider.bounds.center.y < other.bounds.max.y && m_collider.bounds.center.y > other.bounds.min.y)
				{
					
					if(other.bounds.center.x > m_collider.bounds.max.x && m_velocity.x > 0.0f)
					{
						m_velocity.x = 0.0f;	
					}
				}
				
				if(m_collider.bounds.center.y < other.bounds.max.y && m_collider.bounds.center.y > other.bounds.min.y)
				{
					
					if(other.bounds.center.x < m_collider.bounds.max.x && m_velocity.x < 0.0f)
					{
						m_velocity.x = 0.0f;	
					}
				}
			}
			
			
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == "Geometry")
		{
			if(other.gameObject.transform.position.y < transform.position.y)
			{
					m_state = PhysicsState.Falling;		
				
			}
		}
	}
	
	public void AddVelocity(Vector3 direction)
	{
		if(m_state == PhysicsState.Falling)
		{
			direction.y = 0.0f;	
		}
		
		if(m_state == PhysicsState.Grounded && direction.y < 0.0f)
		{
			direction.y = 0.0f;
		}
		
		if(Mathf.Abs(direction.x) > MaxSpeed)
		{
			direction.x = MaxSpeed / (direction.x / Mathf.Abs(direction.x));	
		}
		
		m_velocity += direction;	
	}
}
