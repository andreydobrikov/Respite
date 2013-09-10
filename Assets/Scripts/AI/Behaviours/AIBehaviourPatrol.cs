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
		
		
		m_agent.SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
		
		//m_agent.destination = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.3f);
	}
	
	public override bool Update() 
	{ 
		if(m_agent.remainingDistance == 0.0f)
		{
			m_targetIndex++;
			m_targetIndex = m_targetIndex % m_nodes.Count;
			
			m_origin = m_target;
			
			m_target = m_nodes[m_targetIndex];
			m_agent.SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
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
		
		m_nodes[m_selectedNode].position = EditorGUILayout.Vector3Field("Selected Waypoint", m_nodes[m_selectedNode].position);
		m_nodes[m_selectedNode].sequenceIndex = EditorGUILayout.IntField("Index", m_nodes[m_selectedNode].sequenceIndex);
		
		if(GUILayout.Button("Add Waypoint"))
		{
			m_nodes.Add(new WaypointNode());	
		}
		
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
	
	[SerializeField]
	private List<WaypointNode> m_nodes = new List<WaypointNode>();
	
	private int m_selectedNode = 0;
	
	private WaypointNode m_target = null;
	private WaypointNode m_origin = null;
	private int m_targetIndex = 1;
	
	private GameObject m_player = null;
	private NavMeshAgent m_agent = null;
}
