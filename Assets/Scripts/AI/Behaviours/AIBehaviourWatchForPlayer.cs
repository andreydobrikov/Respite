///////////////////////////////////////////////////////////
// 
// AIBehaviourWatchForPlayer.cs
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

public class AIBehaviourWatchForPlayer : AIBehaviour 
{
	public AIBehaviourWatchForPlayer()
	{
		m_name = "Watch For Player";	
		m_supportTransitions = true;
	}
	
	public override void Start() 
	{
		m_player = GameObject.FindGameObjectWithTag("Player");
	}
	
	public override bool Update() 
	{
		// TODO: Oh dear. Sort this parent name business
		if(Parent.Parent.PlayerInPerceptionRange)
		{
			Quaternion orientation = GetObject().transform.rotation;
			float aiAngle = orientation.eulerAngles.y;
			
			Vector3 position = GetObject().transform.position;
			Vector3 direction = m_player.transform.position - position;
			
			float angle = Mathf.Atan2(direction.x, direction.z);
			
			float diffAngle = Mathf.Acos(Vector3.Dot(direction.normalized, (orientation * Vector3.forward).normalized));
			
			if(diffAngle * Mathf.Rad2Deg < m_viewAngle)
			{
				const int sweepValues = 5;
				
				// TODO: 0.3f should be player collider radius
				// TODO: Aw, man. Trig functions again
				float distanceToPlayer = direction.magnitude;
				
				float targetColliderOffset = Mathf.Atan(0.3f / distanceToPlayer);
				
				float sweepStart = angle - targetColliderOffset;
				float sweepDelta = (targetColliderOffset * 2.0f) / (float)sweepValues;
				
				RaycastHit hitInfo;
				
				for(int i = 0; i < sweepValues; ++i)
				{
					float currentAngle = (sweepStart + (sweepDelta * i)) * Mathf.Rad2Deg;	
					Vector3 rayDirection = Quaternion.Euler(0.0f, currentAngle, 0.0f) * Vector3.forward;
					
					//Debug.DrawLine(position, position + ( ), Color.magenta);
					Debug.DrawRay(position, rayDirection * distanceToPlayer, Color.red);
					if(!Physics.Raycast(position, rayDirection, out hitInfo, distanceToPlayer, ~LayerMask.NameToLayer("LevelGeo")))
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	public override void End() { }
	
	private GameObject m_player = null;
	
	private float m_viewAngle = 180.0f;
}
