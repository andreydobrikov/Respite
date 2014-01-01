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

public class Footsteps : AnimationEventResponder 
{
	public GameObject LeftFoot = null;
	public GameObject RightFoot = null;
	public ObjectPool FootstepPool;
	public ObjectPool FootprintPool;
	public Rigidbody TargetBody;
	public NavMeshAgent TargetAgent;
	
	public override void HandleEvent()
	{
		if(FootstepPool == null || FootprintPool == null || (TargetBody == null && TargetAgent == null))
		{
			Debug.LogWarning("Cannot issue footsteps due to missing parameter-objects");
			return;
		}

		// TODO: Make this related to player move speed
		float speed = TargetBody != null ? TargetBody.velocity.magnitude : TargetAgent.velocity.magnitude;

		GameObject footstep = FootstepPool.ActivateObject();
		GameObject footprint = FootprintPool.ActivateObject();
		
		if(footstep != null)
		{
			footstep.transform.position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);	
			
			NoiseRipple ripple = footstep.GetComponent<NoiseRipple>();
			
			if(ripple != null)
			{
				ripple.maxScale =  speed;
			}
			
			if(footprint != null)
			{
				Vector3 footPosition = m_currentFootIsLeft ? LeftFoot.transform.position : RightFoot.transform.position;

				Vector3 printPosition = new Vector3(footPosition.x, footprint.transform.position.y, footPosition.z);
				footprint.transform.position = printPosition;
				footprint.transform.localRotation = transform.rotation;
				m_currentFootIsLeft = !m_currentFootIsLeft;
			}
		}
	}

	private bool m_currentFootIsLeft = false;
}
