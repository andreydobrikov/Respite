///////////////////////////////////////////////////////////
// 
// PatrolWaypoint.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

public class PatrolWaypoint : MonoBehaviour 
{
	public float TurnSpeed = 0.01f;
	public List<WaypointNode> Waypoints = new List<WaypointNode>();
	
	public PatrolWaypoint()
	{
#if UNITY_EDITOR
		SelectedWaypoint 	= null;
		DragStartPoint		= null;
		Editing				= false;
		HighlightedNode 	= null;
#endif
	}
	
	public WaypointNode CreateWaypoint()
	{
		WaypointNode newWaypoint = ScriptableObject.CreateInstance(typeof(WaypointNode)) as WaypointNode;
		Waypoints.Add(newWaypoint);
		m_maxID++;
		return newWaypoint;
	}
	
	public void DeleteWaypoint(WaypointNode waypoint)
	{
		foreach(var connection in waypoint.m_connections)
		{
			connection.m_connections.Remove(waypoint);	
		}
		
		if(waypoint == DragStartPoint)
		{
			DragStartPoint = null;	
		}
		
		Waypoints.Remove(waypoint);
	}
	
	public bool RouteValid()
	{
		return true;	
	}
	
	void Start ()
	{
		Waypoints.Sort(NodeComparison);
		
		m_origin = Waypoints[0];
		m_target = Waypoints[1];
		
		m_agent = GetComponent<NavMeshAgent>();
		
		m_agent.SetDestination(new Vector3(m_target.position.x, transform.position.y, m_target.position.y));
	}

	void Update ()
	{
		if(m_agent.remainingDistance == 0.0f)
		{
			m_targetIndex++;
			m_targetIndex = m_targetIndex % Waypoints.Count;
			
			m_origin = m_target;
			
			m_target = Waypoints[m_targetIndex];
			m_agent.SetDestination(new Vector3(m_target.position.x, transform.position.y, m_target.position.y));
		}
		
		/*
		switch(m_state)
		{
			case AIState.Walking:
			{
				m_lerpProgress += m_lerpSpeed;
			
				m_lerpRotationProgress += TurnSpeed;
			
				m_lerpRotationProgress = Mathf.Clamp(m_lerpRotationProgress, 0.0f, 1.0f);
			
				float easedProgress = Mathf.Sin(m_lerpRotationProgress * Mathf.PI / 4.0f);
				
				float newAngle = Mathf.LerpAngle(m_originAngle, m_targetAngle, easedProgress);
			
				transform.rotation = Quaternion.Euler(0.0f, 0.0f, newAngle);
		
				Vector3 newPos = Vector2.Lerp(m_origin.position, m_target.position, m_lerpProgress);
				newPos.z = -0.1f;
				transform.position = newPos;
				
				
				if(m_lerpProgress >= 1.0f)
				{
					m_targetIndex++;
					m_targetIndex = m_targetIndex % Waypoints.Count;
					
					m_origin = m_target;
					
					m_target = Waypoints[m_targetIndex];
					
					Vector2 diff = m_target.position - m_origin.position;
					
					m_lerpSpeed = diff.magnitude;
					m_lerpSpeed = 0.01f / m_lerpSpeed;
					
					m_lerpProgress = 0.0f;
					
					// TEMP: Rotate
					//float newRotation = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
					
					//transform.rotation = Quaternion.Euler(0.0f, 0.0f, -newRotation);
					m_state = AIState.Holding;
				
					m_holdProgress = 0.0f;
					m_holdTime = m_origin.HoldTime;
						
					m_lerpRotationProgress = 0.0f;
					m_originAngle = m_targetAngle;
				
					Vector2 direction = m_target.position - m_origin.position;
					m_targetAngle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
				}
				break;
			}

		}
		*/
	}
	
	private static int NodeComparison(WaypointNode n0, WaypointNode n1)
	{
		if(n0.sequenceIndex == n1.sequenceIndex)
		{
			return 0;
		}
		
		if(n0.sequenceIndex > n1.sequenceIndex)
		{
			return 1;	
		}
		
		return -1;
	}
	
	private WaypointNode m_target;
	private WaypointNode m_origin;
			
	private int m_targetIndex = 1;
	
	
	private NavMeshAgent m_agent = null;
	
#if UNITY_EDITOR
	
	public WaypointNode 	SelectedWaypoint { get; set; }
	public WaypointNode 	DragStartPoint { get; set; }
	public bool 			Editing { get; set; }	
	public Vector2 			LastDragPos { get; set; }
	public WaypointNode 	HighlightedNode { get; set; }
	
#endif
	
	private enum AIState
	{
		Walking,
		Holding,
		Turning
	}
	
	private int m_maxID = 0;
} 


