///////////////////////////////////////////////////////////
// 
// AIBehaviourPatrol.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class AIBehaviourPatrol : AIBehaviour 
{
	public AIBehaviourPatrol() 
	{
		m_name = "patrol behaviour";
	}
	
	public override void Start() 
	{
		m_agent = GetObject().GetComponent<NavMeshAgent>();
		m_player = GameObject.FindGameObjectWithTag("Player");
		
		if(m_agent == null)
		{
			Debug.LogError("AIBehaviourPatrol requires NavMeshAgent component");
		}
		
		m_nodes.Sort(NodeComparison);
		
		m_origin = m_nodes[0];
		m_target = m_nodes[1];
		
		m_targetIndex = 0;
		
		m_agent.SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
		
		//m_agent.destination = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.3f);
		
		 m_activeState = PatrolState.Routing;
	}
	
	public override bool Update() 
	{ 
		
		switch(m_activeState)
		{
			case PatrolState.Routing:
			{
				Door door = RayCastDoor();
				if(door != null)
				{
					m_cachedTarget = m_agent.destination;
					Debug.Log("Travelling through door");
					m_activeState = PatrolState.DoorFound;
					m_door = door;
				}
				
				if(m_agent.remainingDistance == 0.0f)
				{
					m_targetIndex++;
					
					if(m_targetIndex >= m_nodes.Count)
					{
						if(m_loop)
						{
							m_targetIndex = m_targetIndex % m_nodes.Count;
						}
						else
						{
							m_targetIndex = 0;
							return true;
						}
					}
					
					m_origin = m_target;
					
					m_target = m_nodes[m_targetIndex];
					m_agent.SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
				}	
				break;
			}
			
			case PatrolState.DoorFound:
			{
				if(m_door.State == Door.DoorState.Closed)
				{
					m_door.Open(GetObject());
				
				}
				else if(m_door.State == Door.DoorState.Open)
				{
					Door door = RayCastDoor();
					// if the player still wants to go through the door once it's open, point them to the door's end
					if(door != null && door == m_door)
					{
						Vector3 pivot = m_door.PivotPosition;
						Vector3 direction = m_door.DoorDirection;
						
						m_agent.destination = pivot + (direction * 1.4f);
	
						m_activeState = PatrolState.RoutingAroundDoor;
					}
					else
					{
						// Otherwise, return them to their route
						m_agent.destination = m_cachedTarget;
						m_activeState = PatrolState.Routing;
					}
				}
				//m_activeState = PatrolState.Routing;
				break;
			}

			case PatrolState.RoutingAroundDoor:
			{
				if (m_agent.remainingDistance == 0.0f)
				{
					m_agent.destination = m_cachedTarget;
					m_activeState = PatrolState.Routing;
				}
				break;			
			}
		}
		
		return false;
	}
	
	public override void End() 
	{
		m_agent.destination = GetObject().transform.position;
		
	}
	
#if UNITY_EDITOR
	
	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical((GUIStyle)("Box"));
			
		if(m_selectedNode < m_nodes.Count)
		{
			m_nodes[m_selectedNode].position = EditorGUILayout.Vector3Field("Selected Waypoint", m_nodes[m_selectedNode].position);
			m_nodes[m_selectedNode].sequenceIndex = EditorGUILayout.IntField("Index", m_nodes[m_selectedNode].sequenceIndex);
		}
		
		if(GUILayout.Button("Add Waypoint"))
		{
			m_nodes.Add(new WaypointNode());	
		}
		
		m_loop = GUILayout.Toggle(m_loop, "Loop");
		
		m_supportTransitions = !m_loop;
		
		GUILayout.EndVertical();
	}
	
	public override void OnSceneGUI()
	{
		Handles.color = Color.green;
		Handles.DrawLine(Vector3.zero, Vector3.one);
		
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		
		int controlIndex = 0;
		foreach(var node in m_nodes)
		{
			int hotControl = GUIUtility.hotControl;
			
			if(controlIndex == m_selectedNode)
			{
				Handles.color = Color.red;
			}
			
			Handles.SphereCap(0, node.position, Quaternion.identity, 0.5f);
			
			Handles.color = Color.green;
			
			node.position = Handles.PositionHandle(node.position, Quaternion.identity);
			
			
			if(GUIUtility.hotControl != hotControl)
			{
				Debug.Log("Changed");	
				m_selectedNode = controlIndex;
			}
			
			builder = new System.Text.StringBuilder();
			builder.Append(node.sequenceIndex);
			builder.Append('\n');
			
			Handles.Label(node.position + new Vector3(1.0f, 0.0f, -1.0f), builder.ToString(), (GUIStyle)("Box"));
			
			//Handles.DrawSolidDisc(new Vector3(node.position.x, -1.0f, node.position.y), Vector3.up, 3.0f);
			controlIndex++;
		}
	}
#endif
	
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
	
	[SerializeField]
	private List<WaypointNode> m_nodes = new List<WaypointNode>();
	
	[SerializeField]
	private bool m_loop = false;
	
	private int m_selectedNode = 0;
	
	private WaypointNode m_target = null;
	private WaypointNode m_origin = null;
	private int m_targetIndex = 1;
	
	private GameObject m_player = null;
	private NavMeshAgent m_agent = null;
	
	private PatrolState m_activeState = PatrolState.Routing;
	private Vector3 m_cachedTarget = Vector3.zero;
	private Door m_door = null;
			
	private enum PatrolState
	{
		Routing,
		DoorFound,
		RoutingAroundDoor
	}
	
}
