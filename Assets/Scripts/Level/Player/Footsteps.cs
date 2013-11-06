///////////////////////////////////////////////////////////
// 
// Footsteps.cs
//
// What it does: Christ alive
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Footsteps : MonoBehaviour 
{
	public ObjectPool FootstepPool;
	public ObjectPool FootprintPool;
	public Rigidbody TargetBody;
	public NavMeshAgent TargetAgent;
	
	public float walkFrequency = 1.0f;
	public float runFrequency = 0.3f;
	public float footWidth		= 0.2f;
	
	void Start ()
	{
		m_target = walkFrequency;
		m_footprintTarget = walkFrequency / 2.0f;
	}

	void Update ()
	{
		if(FootstepPool == null || FootprintPool == null || (TargetBody == null && TargetAgent == null))
		{
			Debug.LogWarning("Cannot issue footsteps due to missing parameter-objects");
			return;
		}
		
		// TODO: What a joke. Work out what the player's actual max-speed is, rather than bunging 4.5f in here.
		float speed = 0.0f;
		
		if(TargetBody != null)
		{
			speed =	(TargetBody.velocity.magnitude / 4.5f);
		}
		else
		{
			speed = (TargetAgent.velocity.magnitude / 1.5f);	
		}
		
		m_countDownTimer += Time.deltaTime;
		m_footprintTimer += Time.deltaTime * 2.0f;
		
		if(speed > 0.1)
		{
			// TODO: The max and mins here should be exactly matched to the ratio between walk and run speeds
			// in CharacterController2D!
			m_target = Mathf.Lerp(runFrequency, walkFrequency, 1.0f - speed); 
			if(m_countDownTimer >= m_target)
			{
				GameObject footstep = FootstepPool.ActivateObject();
				
				
				if(footstep != null)
				{
					footstep.transform.position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);	
					
					NoiseRipple ripple = footstep.GetComponent<NoiseRipple>();
				
					if(ripple != null)
					{
						ripple.maxScale = 8.0f * speed;
					}
				}
				
				
				
				m_countDownTimer = 0.0f;
			}
			
			if(m_footprintTimer >= m_target)
			{
				GameObject footprint = FootprintPool.ActivateObject();
				
				if(footprint != null)
				{
					footprint.transform.position = new Vector3(transform.position.x + (m_leftFoot ? -footWidth : footWidth), footprint.transform.position.y, transform.position.z);
					footprint.transform.localRotation = transform.rotation;
					m_leftFoot = !m_leftFoot;
				}
				m_footprintTimer = 0.0f;
			}
		}
		else
		{
			//m_countDownTimer = 0.0f;	
		}
		
		
	}
	private float m_target = 0.0f;
	private float m_countDownTimer = 0.0f;
	
	private float m_footprintTarget = 0.0f;
	private float m_footprintTimer = 0.0f;
	private bool m_leftFoot = false;
}
