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
		m_name = "Patrol";
		m_requiredTaskPaths.Add("patrol_task");
	}

	public override void Start()
	{
		m_parentAI.Blackboard.AddEntry<float>("nav_idle_time", 3.0f); 
		m_navTargetHash = m_parentAI.Blackboard.AddEntry<Vector3>("nav_target_position", Vector3.one); 

		AITask patrolTask = null;
		if(m_tasks.TryGetValue("patrol_task", out patrolTask))
		{
			if(patrolTask.Result == AITaskResult.Idle)
			{
				// Update the destination
				m_parentAI.Blackboard.SetEntry<Vector3>(m_navTargetHash, GameObject.FindGameObjectWithTag("Player").transform.position);

				m_parentAI.PushTask(patrolTask); 
			}
		}
	}

	public override void Update()
	{
	
	}

	public override void Shutdown()
	{
			
	}

#if UNITY_EDITOR
	public override void OnSceneGUI()
	{
		AITask patrolTask = null;
		if(m_tasks.TryGetValue("patrol_task", out patrolTask))
		{
			if(patrolTask.Result != AITaskResult.Idle)
			{
				GUI.enabled = false;
			}

			if(GUI.Button(new Rect(10, Screen.height / 2 - 20, 100, 40), "Run Patrol"))
			{
				// Update the destination
				m_parentAI.Blackboard.SetEntry<Vector3>(m_navTargetHash, GameObject.FindGameObjectWithTag("Player").transform.position);

				m_parentAI.PushTask(patrolTask);
			}
			GUI.enabled = true;
		}


	}
#endif

	private AITask m_patrolTask = null;

	private int m_navTargetHash = 0;

}

/*
public class AIBehaviourPatrol : AIBehaviourNavigationBase 
{
	public AIBehaviourPatrol() 
	{
		m_name = "patrol behaviour";
		m_destinationReached = WaypointReached;
	}
	
	public override void NavStart() 
	{
		if(m_nodes.Count <= 0)
		{
			m_targetIndex = -1;
			Debug.LogWarning("No waypoints set");
			return;
		}

		m_nodes.Sort(NodeComparison);

		m_target = m_nodes[0];
		
		m_targetIndex = 0;
		
		SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
	}
	
	public override bool NavUpdate() 
	{ 
		return false;
	}
	
	public override void NavEnd() {	}
	
	protected bool WaypointReached()
	{
		if(m_targetIndex == -1)
		{
			return false;
		}

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
		
		m_target = m_nodes[m_targetIndex];
		
		SetDestination(new Vector3(m_target.position.x, m_target.position.y, m_target.position.z));
		
		return false; 
	}
	
#if UNITY_EDITOR
	
	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical((GUIStyle)("Box"), GUILayout.Width(300));
			
		if(m_selectedNode < m_nodes.Count)
		{
			m_nodes[m_selectedNode].position = EditorGUILayout.Vector3Field("Selected Waypoint", m_nodes[m_selectedNode].position);
			m_nodes[m_selectedNode].sequenceIndex = EditorGUILayout.IntField("Index", m_nodes[m_selectedNode].sequenceIndex);
		}

		List<WaypointNode> toDelete = new List<WaypointNode>();

		int i = 0;
		foreach(var node in m_nodes)
		{
			GUILayout.BeginHorizontal();

			node.sequenceIndex = EditorGUILayout.IntField("Index", node.sequenceIndex);

			if(GUILayout.Button("goto", GUILayout.Width(40)))
			{
				SceneView.lastActiveSceneView.pivot = new Vector3(node.position.x, node.position.y + 3.0f, node.position.z);
				SceneView.lastActiveSceneView.Repaint();
				m_selectedNode = i;
			}

			if(GUILayout.Button("x", GUILayout.Width(20)))
			{
				if(EditorUtility.DisplayDialog("Delete Node?", "Delete node?", "ok", "cancel"))
				{
					toDelete.Add(node);
				}
			}

			GUILayout.EndHorizontal();
			i++;
		}

		foreach(var node in toDelete)
		{
			m_nodes.Remove(node);
		}

		if(GUILayout.Button("Add Waypoint"))
		{
			WaypointNode newNode = ScriptableObject.CreateInstance<WaypointNode>();
			m_nodes.Add(newNode);	
			newNode.position = GetGameObject().transform.position;
			newNode.position.x += 2.0f;
			newNode.position.y = 0.3f;
		}
		
		m_loop = GUILayout.Toggle(m_loop, "Loop");

		UpdateSupportsTransitions(!m_loop);
		
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
			
			Vector3 newPosition = Handles.PositionHandle(node.position, Quaternion.identity);
			
			if(newPosition != node.position)
			{
				node.position = newPosition;
				EditorUtility.SetDirty(this);
			}
			
			if(GUIUtility.hotControl != hotControl) 
			{
				m_selectedNode = controlIndex;
			}
			
			builder = new System.Text.StringBuilder();
			builder.Append(node.sequenceIndex);
			builder.Append('\n');
			
			Handles.Label(node.position + new Vector3(1.0f, 0.0f, -1.0f), builder.ToString(), (GUIStyle)("Box"));
			
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
	
	[SerializeField]
	private bool m_loop = false;
	
	private int m_selectedNode = 0;
	
	private WaypointNode m_target = null;
	private int m_targetIndex = 1;
	
	private enum PatrolState
	{
		Patrolling,
		Holding
	}
}
*/
