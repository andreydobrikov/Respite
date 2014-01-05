///////////////////////////////////////////////////////////
// 
// AIBehaviourNavigationBase.cs
//
// What it does: Provides a base class for AI that involves navigation.
//
// Notes: Navigation is relatively involved. Doors, floor transitions, etc all require custom handling.
//		  This class exists to save me replicating all the guff for each behaviour.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
/*
[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIBehaviourNavigationBase : AIBehaviour 
{
	// Called when a requested destination is reached.
	public delegate bool DestinationReachedHandler();
	
	public override void Start()
	{
		m_animator 	= m_parentState.Parent.GetComponentInChildren<Animator>();
		m_agent 	= GetGameObject().GetComponent<NavMeshAgent>();

		if(m_agent == null)
		{
			Debug.LogError("AIBehaviourPatrol requires NavMeshAgent component");
		}
		
		m_agent.autoTraverseOffMeshLink = false;
		
		m_activeState = AINavigationState.Routing;
		
		NavStart();
	}
	
	public override bool Update()
	{
		bool transition = false;
		
		if(m_door != null)
		{
			Debug.DrawLine(GetGameObject().transform.position, m_door.transform.position + new Vector3(0.0f, 1.0f, 0.0f), Color.magenta);	
		}
		
		switch(m_activeState)
		{
			case AINavigationState.Routing:
			{
				if(m_agent.pathPending == true)
				{
					if(m_showDebugOutput)
					{
						Debug.Log(this.Name + ": Path Pending");
					}

					break;	
				}

				if(m_animator != null)
				{
					m_animator.SetFloat("speed", m_agent.speed);
				}
			
				Door door = RayCastDoor();
			
				// If there's a door, switch to the appropriate state
				if(door != null)
				{
					m_activeState = AINavigationState.DoorFound;
					m_door = door;
				}
				
				// If the destination is reached, inform the derived behaviour
				if(m_agent.remainingDistance == 0.0f)
				{
					if(m_destinationReached != null)
					{
						transition = m_destinationReached();
					}
					else
					{
						return true;	
					}
				}
			
				// Handle floor transitions
				if(m_agent.isOnOffMeshLink)
				{
					m_agent.transform.position = m_agent.currentOffMeshLinkData.endPos;
					m_agent.CompleteOffMeshLink();
				}
			
				break;
			}

			case AINavigationState.Turning:
			{
				if(!m_turningPathCalculated)
				{
					NavMeshPath path = new NavMeshPath();
					if(!m_agent.CalculatePath(m_destination, path))
					{
						Debug.LogWarning("Couldn't route to destination");
						m_destinationReached();
					}
					else
					{
						m_turnTarget = path.corners[1];
						m_turningPathCalculated = true;
					}
					
				}
				else
				{
					Vector3 directionToTarget = m_turnTarget - GetGameObject().transform.position;
					Debug.DrawLine(GetGameObject().transform.position, GetGameObject().transform.position + directionToTarget, Color.magenta);
					float rotation = GetGameObject().transform.rotation.eulerAngles.y;	
					float newRotation = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
					float delta = Mathf.DeltaAngle(rotation, newRotation);

					if(Mathf.Abs(delta) < 50.0f)
					{
						m_activeState = AINavigationState.Routing;
						m_agent.SetDestination(m_destination);
					}

					Quaternion targetRotation = Quaternion.Euler(0.0f, newRotation, 0.0f);					
					Quaternion currentRotation = GetGameObject().transform.rotation;
					GetGameObject().transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, 3.5f);
				}

				break;
			}
			
			case AINavigationState.DoorFound:
			{
				m_doorOpenHoldTimer -= Time.deltaTime;
				if(m_door.State == Door.DoorState.Closed)
				{
					m_door.Open(GetGameObject());
					m_doorOpenHoldTimer = m_doorOpenHoldingTime;
					m_agent.destination = m_agent.transform.position;
				
				}
				else if (m_doorOpenHoldTimer <= 0.0f && m_door.State == Door.DoorState.Open)
				{
					// Otherwise, return them to their route
					m_agent.destination = m_destination;
					m_activeState = AINavigationState.Routing;
				}
				break;
			}

		}
		
		transition |= NavUpdate();
		return transition;
	}
	
	public override void End()
	{
		m_agent.destination = GetGameObject().transform.position;
		
		NavEnd();
	}

	/// <summary>
	/// Sets the destination of the AI and halts the current processing.
	/// </summary>
	protected void SetDestination(Vector3 destination)
	{
		m_destination = destination;

		// Stop the navmeshagent where it is.
		m_agent.destination = GetGameObject().transform.position;

		// Clear the turning path so that the turning state knows to recalculate the next corner.
		m_turningPathCalculated = false;

		// Switch the state to rotate towards the new goal.
		m_activeState = AINavigationState.Turning;

	}
	
	private Door RayCastDoor()
	{
		Debug.DrawLine(GetGameObject().transform.position, GetGameObject().transform.position + m_agent.velocity, Color.green);
				
		Vector3 direction = m_agent.steeringTarget - GetGameObject().transform.position;
		
		Debug.DrawRay(GetGameObject().transform.position, direction, Color.yellow);
	
		RaycastHit hitInfo;
		if(Physics.Raycast(GetGameObject().transform.position, direction, out hitInfo, direction.magnitude, ~LayerMask.NameToLayer("Interactive")))
		{
			Door door = hitInfo.collider.gameObject.GetComponent<Door>();
			if(door != null)
			{	
				return door;
			}
		}
	
		if(Physics.Raycast(GetGameObject().transform.position, m_agent.velocity, out hitInfo, 1.0f, ~LayerMask.NameToLayer("Interactive")))
		{
			Door door = hitInfo.collider.gameObject.GetComponent<Door>();
			if(door != null)
			{
				return door;
			}
		}
			
		return null;
	}

#region Abstract Methods

	public abstract void NavStart();
	public abstract bool NavUpdate();
	public abstract void NavEnd();

#endregion

#region Protected Members

	protected DestinationReachedHandler m_destinationReached 	= null;

#endregion

#region Private Members

	private const float m_doorOpenHoldingTime					= 1.0f; // How long to wait before re-pathing once a door is opened. TODO: Attach this to the door open speed?

	private NavMeshAgent m_agent 								= null;
	private Animator m_animator									= null;

	private AINavigationState m_activeState 					= AINavigationState.Routing;	// The current state of the navigation state-machine.
	private Vector3 m_destination								= Vector3.zero;					// The current routing target. TODO: This may make ^^^this^^^ irrelevant.
	private Door m_door 										= null;							// The door being handled in the DoorFound state.
	private float m_doorOpenHoldTimer							= 0.0f;							// Countdown when waiting for a door to open.
	private Vector3 m_turnTarget								= Vector3.zero;			// Path for use when in the turning state. Allows calculation of the target corner
	private bool m_turningPathCalculated						= false;

#if UNITY_EDITOR
	private bool m_showDebugOutput								= false;						// Show debug output. 
#endif

#endregion
}

public enum AINavigationState
{
	Routing,
	DoorFound,
	Turning,
}
*/