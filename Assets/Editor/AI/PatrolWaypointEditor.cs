///////////////////////////////////////////////////////////
// 
// PatrolWaypointEditor.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(PatrolWaypoint))]
public class PatrolWaypointEditor : Editor 
{
	public void OnSceneGUI()
	{
		PatrolWaypoint patrol = (PatrolWaypoint)target;
		Vector3 offset = new Vector3(0.0f, 0.0f, -1.0f);
		
		Vector3 lastPoint = Vector3.zero;
		
		Handles.color = Color.green;
		
		int handleID = 1;
		
		GUIStyle style = (GUIStyle)("textarea");
		
		WaypointNode selected = null;
		
		foreach(var point in patrol.Waypoints)
		{
			float handleSize = HandleUtility.GetHandleSize((Vector3)point.position + offset) / 8.0f;
			
			
			
			if(patrol.HighlightedNode == point)
			{
				Handles.color = Color.blue;
			}
			
			if(patrol.SelectedWaypoint == point)
			{
				Handles.color = Color.yellow;	
			}
			
			Vector2 newPos = Handles.Slider2D(handleID, point.position, Vector3.forward, Vector3.right, Vector3.up, handleSize, Handles.SphereCap, handleSnap);
			
			Handles.color = Color.green;
			
			if(!patrol.Editing)
			{
				point.position = newPos;	
			}
			
			if(GUIUtility.hotControl == handleID)
			{
				patrol.SelectedWaypoint = point;
				selected = point;
			}
			
			foreach(var connection in point.m_connections)
			{
				Handles.DrawLine(point.position, connection.position);	
			}
			
			if(patrol.SelectedWaypoint == point)
			{
				
				style.wordWrap = false;
				
				System.Text.StringBuilder builder = new System.Text.StringBuilder("Position: " + point.position.x.ToString("0.00") + ", " + point.position.y.ToString("0.00"));
				
				Handles.Label(point.position - new Vector2(0.0f, 0.2f), builder.ToString(), style);
			}
			
			if(!patrol.Editing)
			{
				Handles.Label(point.position + new Vector2(-0.1f, 0.4f), point.sequenceIndex.ToString(), style);
			}
			
			handleID++;
		}
		
		if(selected != null && patrol.Editing) 
		{
			if(patrol.DragStartPoint != null)
			{
				if(patrol.DragStartPoint != selected) 
				{
					if(!selected.m_connections.Contains(patrol.DragStartPoint))
					{
						selected.m_connections.Add(patrol.DragStartPoint);
						patrol.DragStartPoint.m_connections.Add(selected);
					}
				}
			}
			patrol.DragStartPoint = selected;
		}
		
		if(patrol.Editing)
		{
			if(Event.current.button == 1)
			{
				patrol.DragStartPoint = null;	
			}
			
			if(patrol.DragStartPoint != null )
			{
				Vector2 mousePos = Event.current.mousePosition;
				mousePos.y = Camera.current.pixelHeight - mousePos.y;
				Ray ray = Camera.current.ScreenPointToRay(mousePos);
				
				Plane groundPlane = new Plane(Vector3.back, Vector3.zero);
				
				float rayDistance;
				
				groundPlane.Raycast(ray, out rayDistance);
				
				Vector3 pos = ray.GetPoint(rayDistance);
				
				pos.z = -2.0f;
				patrol.LastDragPos = pos;
				
				Vector2 direction = patrol.LastDragPos - patrol.DragStartPoint.position;
				
				int layer = LayerMask.NameToLayer("LevelGeo");
				
				Handles.color = Color.green;
				if(Physics.Raycast(patrol.DragStartPoint.position, direction.normalized, direction.magnitude, 1 << layer))
				{
					Handles.color = Color.red;
				}
				
				Handles.DrawLine((Vector3)patrol.DragStartPoint.position, patrol.LastDragPos);
			}
		}
		
		SceneView.RepaintAll();
		
		ShowSceneMenu();
	}
	
	private void ShowSceneMenu()
	{
		PatrolWaypoint patrol = (PatrolWaypoint)target;
		
		GUILayout.BeginArea(new Rect(10, 10, 200, 100));
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		GUI.contentColor = Color.black;
		
		GUILayout.Label("Editing Links: " + (patrol.Editing ? "True" : "False"));
		
		if(GUILayout.Button("Toggle Editing Links"))
		{
			patrol.Editing = !patrol.Editing;	
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
	}
	
	public override void OnInspectorGUI()
	{
		PatrolWaypoint patrol = (PatrolWaypoint)target;
		
		patrol.TurnSpeed = EditorGUILayout.FloatField("Turn Speed", patrol.TurnSpeed);
		
		if(patrol.SelectedWaypoint != null)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			patrol.SelectedWaypoint.sequenceIndex = EditorGUILayout.IntField("Sequence Index", patrol.SelectedWaypoint.sequenceIndex);
			patrol.SelectedWaypoint.HoldTime = EditorGUILayout.FloatField("Hold Time", patrol.SelectedWaypoint.HoldTime);
			
			GUILayout.Label(patrol.SelectedWaypoint.position.x + ", " + patrol.SelectedWaypoint.position.y);
			
			if(GUILayout.Button("Delete"))
			{
				patrol.DeleteWaypoint(patrol.SelectedWaypoint);	 
			}
			
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			List<WaypointNode> toDelete = new List<WaypointNode>();
			
			foreach(var connection in patrol.SelectedWaypoint.m_connections)
			{
				GUILayout.BeginHorizontal();
				
				bool highlighted = EditorGUILayout.Toggle(connection == patrol.HighlightedNode, GUILayout.Width(30));
				
				if(highlighted)
				{
					patrol.HighlightedNode = connection;	
				}
				
				GUILayout.Label("Test");
				if(GUILayout.Button("Delete"))
				{
					toDelete.Add(connection);	
				}
				
				GUILayout.EndHorizontal();
			}
			
			foreach(var deadLink in toDelete)
			{
				deadLink.m_connections.Remove(patrol.SelectedWaypoint);
				patrol.SelectedWaypoint.m_connections.Remove(deadLink);
			}
			
			GUILayout.EndVertical();
			
			
			
			GUILayout.EndVertical();
		}
		
		if(GUILayout.Button("Random points"))
		{
			int id = 0;
			patrol.Waypoints.Clear();
			
			WaypointNode lastPoint = null;
			for(int i = 0; i < 5; i++)
			{
				WaypointNode point = patrol.CreateWaypoint();
				
				point.sequenceIndex = id;
				
				float x = Random.Range(-5.0f, 5.0f);
				float y = Random.Range(-5.0f, 5.0f);
				
				if(lastPoint != null)
				{
					point.m_connections.Add(lastPoint);
					lastPoint.m_connections.Add(point);
				}
				
				point.position = patrol.transform.position + new Vector3(x, y, 0.0f);
				
				lastPoint = point;
				id++;
			}
			
			patrol.Waypoints[0].m_connections.Add(lastPoint);
			lastPoint.m_connections.Add(patrol.Waypoints[0]);
		}
		
		if(GUILayout.Button("Add Waypoint"))
		{
			WaypointNode newWaypoint = patrol.CreateWaypoint();
			
			if(patrol.Waypoints.Count > 1) 
			{
				Vector2 lastPos = patrol.Waypoints[patrol.Waypoints.Count - 2].position;
				newWaypoint.position = lastPos + new Vector2(1.0f, 0.0f);
				
			}
			else
			{
				newWaypoint.position = patrol.transform.position + new Vector3(1.0f, 0.0f, 0.0f);	
			}
		}
	}
	
	private static Vector2 handleSnap = new Vector2(0.1f, 0.1f);
}
