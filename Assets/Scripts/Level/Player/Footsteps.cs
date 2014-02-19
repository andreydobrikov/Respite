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
    public LayerMask RaycastFootstepMask;
	
	public override void HandleEvent()
	{
        //float timeDiff = Time.time - m_lastTime;
        
      //  m_lastTime = Time.time;
        
       // Debug.Log("Time since last footstep: " + timeDiff);

		if(FootstepPool == null || FootprintPool == null || (TargetBody == null && TargetAgent == null))
		{
			Debug.LogWarning("Cannot issue footsteps due to missing parameter-objects");
			return;
		}

		// TODO: Make this related to player move speed
		float speed = TargetBody != null ? TargetBody.velocity.magnitude : TargetAgent.velocity.magnitude;

		GameObject footstep = FootstepPool.ActivateObject();
		GameObject footprint = FootprintPool.ActivateObject();
		
        Vector3 footPosition = m_currentFootIsLeft ? LeftFoot.transform.position : RightFoot.transform.position;

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
				Vector3 printPosition = new Vector3(footPosition.x, footprint.transform.position.y, footPosition.z);
				footprint.transform.position = printPosition;
				footprint.transform.localRotation = transform.rotation;
				m_currentFootIsLeft = !m_currentFootIsLeft; 
			}
		}

        // Deal with making an ear-hear-noise-screech
        // TODO: The 5.0f here should be the difference between floors, but it doesn't really matter
        RaycastHit hitInfo;
        if(Physics.Raycast(footPosition + Vector3.up, -Vector3.up, out hitInfo, 5.0f, RaycastFootstepMask))
        {
			FloorFootstep surface = hitInfo.collider.gameObject.GetComponent<FloorFootstep>();

            if(surface != null)
            {
                if(surface.SurfaceAudioSource != null)
                {
                    Debug.Log("PLAYING FOOTSTEP");
                    surface.SurfaceAudioSource.Play();
                }
            }
        }

	}

  //  private float m_lastTime = 0.0f;
	private bool m_currentFootIsLeft = false;
}
