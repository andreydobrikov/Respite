using System;
using UnityEngine;
using System.Collections;

public class Camera : WorldObject 
{
	public float range = 2.0f;
	public float fov_degrees = 35.0f;
	public float rotation = 0.0f;
	public float rotationSpeed = 0.5f;
	public float maxRotation_degrees = 90.0f;
	private float m_initialRotation = 0.0f;
	
	public event EventHandler StateChanged;
	
	public enum TargetState
	{
		Spotted,
		Searching
	}
	
	public TargetState GetState()
	{
		return m_state;	
	}
	
	
	protected TargetState m_state = TargetState.Searching;
	
	Camera()
	{
			
	}
	
	protected override void OnStart()
	{
		m_initialRotation = rotation;
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, m_initialRotation);	
	}
	
	
	private bool right = false;
	void FixedUpdate()
	{
		//return;
		if(right)
		{
			if(rotation < maxRotation_degrees)
			{
				rotation += rotationSpeed;
			}
			else
			{
				right = !right;	
			}
		}
		else
		{
			if(rotation > -maxRotation_degrees)
			{
				rotation -= rotationSpeed;
			}
			else
			{
				right = !right;	
			}
		}
		
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, m_initialRotation + rotation);
	}
	
	private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) 
	{
		int state = 0;
        if (stream.isWriting) 
		{
			if(m_stateChanged)
			{
				m_stateChanged = false;
            	state = (int)m_state;
            	stream.Serialize(ref state);
			}
        }
		else 
		{
            stream.Serialize(ref state);
            TargetState newState = (TargetState)state;
			if(newState != m_state)
			{
				m_state = newState;
				if(StateChanged != null)
				{
					StateChanged(this, null);
				}
			}
        }
    }
	
	public void ChangeState(TargetState state)
	{
		if(state != m_state)
		{
			m_state = state;
			m_stateChanged = true;
			if(StateChanged != null)
			{
				StateChanged(this, null);
			}
		}
	}
	
	void OnDrawGizmos()
	{
		Quaternion currentRotation = Quaternion.Euler(0.0f, 0.0f, m_initialRotation + rotation);
		Gizmos.matrix = Matrix4x4.TRS(transform.position, currentRotation, transform.localScale);
		
		float halfWidth = range * Mathf.Tan((fov_degrees/ 2.0f) * Mathf.Deg2Rad) ;
		
		float extentsHalfWidth = range * Mathf.Tan((fov_degrees/ 2.0f + maxRotation_degrees) * Mathf.Deg2Rad) ;
		
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-halfWidth, range, 0.0f));
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(halfWidth, range, 0.0f));
		
		Gizmos.color = Color.gray;
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-extentsHalfWidth, range, 0.0f));
		Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(extentsHalfWidth, range, 0.0f));
	}
	
	private bool m_stateChanged = false;
}
