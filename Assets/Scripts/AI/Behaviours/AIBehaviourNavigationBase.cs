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

[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIBehaviourNavigationBase : AIBehaviour 
{
	// Called when a requested destination is reached.
	public delegate bool DestinationReachedHandler();
	
	public override void Start()
	{
		m_agent = GetObject().GetComponent<NavMeshAgent>();

	//	string extraOutputSetting = Settings.Instance.GetSetting("ai_navigation_extra_output");
		//m_showDebugOutput = extraOutputSetting != null ? (extraOutputSetting == "true") : false;
		
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
			Debug.DrawLine(GetObject().transform.position, m_door.transform.position + new Vector3(0.0f, 1.0f, 0.0f), Color.magenta);	
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
			
				Door door = RayCastDoor();
			
				// If there's a door, switch to the appropriate state
				if(door != null)
				{
					m_activeState = AINavigationState.DoorFound;
					m_cachedTarget = m_agent.destination;
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
			
			case AINavigationState.DoorFound:
			{
				m_doorOpenHoldTimer -= Time.deltaTime;
				if(m_door.State == Door.DoorState.Closed)
				{
					m_door.Open(GetObject());
					m_doorOpenHoldTimer = m_doorOpenHoldingTime;
					m_agent.destination = m_agent.transform.position;
				
				}
				else if (m_doorOpenHoldTimer <= 0.0f && m_door.State == Door.DoorState.Open)
				{
					
					
						// Otherwise, return them to their route
						m_agent.destination = m_cachedTarget;
						m_activeState = AINavigationState.Routing;
					
				}
				//m_activeState = PatrolState.Routing;
				break;
			}

		}
		
		transition |= NavUpdate();
		return transition;
	}
	
	public override void End()
	{
		m_agent.destination = GetObject().transform.position;
		
		NavEnd();
	}
	
	protected void SetDestination(Vector3 destination)
	{
		m_requestedRoute = destination;
		
		if(m_activeState == AINavigationState.Routing)
		{
			m_agent.destination = m_requestedRoute;
		}
	}
	
	private Door RayCastDoor()
	{
		Debug.DrawLine(GetObject().transform.position, GetObject().transform.position + m_agent.velocity, Color.green);
				
		Vector3 direction = m_agent.steeringTarget - GetObject().transform.position;
		
		Debug.DrawRay(GetObject().transform.position, direction, Color.yellow);
	
		RaycastHit hitInfo;
		if(Physics.Raycast(GetObject().transform.position, direction, out hitInfo, direction.magnitude, ~LayerMask.NameToLayer("Interactive")))
		{
			Door door = hitInfo.collider.gameObject.GetComponent<Door>();
			if(door != null)
			{	
				return door;
			}
		}
	
		if(Physics.Raycast(GetObject().transform.position, m_agent.velocity, out hitInfo, 1.0f, ~LayerMask.NameToLayer("Interactive")))
		{
			Door door = hitInfo.collider.gameObject.GetComponent<Door>();
			if(door != null)
			{
				return door;
			}
		}
			
		return null;
	}
	
	public abstract void NavStart();
	public abstract bool NavUpdate();
	public abstract void NavEnd();

	protected const float m_doorOpenHoldingTime					= 1.0f; // How long to wait before re-pathing once a door is opened. TODO: Attach this to the door open speed?
	
	protected DestinationReachedHandler m_destinationReached 	= null;
	
	private NavMeshAgent m_agent 								= null;
	private AINavigationState m_activeState 					= AINavigationState.Routing;
	private Vector3 m_cachedTarget 								= Vector3.zero;
	private Vector3 m_requestedRoute							= Vector3.zero;
	private Door m_door 										= null;
	private bool m_showDebugOutput								= false;
	private float m_doorOpenHoldTimer							= 0.0f;
}

public enum AINavigationState
{
	Routing,
	DoorFound,
	RoutingAroundDoor
}
