using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[Serializable]
public class LayoutNode : ScriptableObject
{
	public enum EditType
	{
		Move,
		Link
	}
	
	public void AddConnection(LayoutNode other)
	{
		if(m_connections.Count < m_maxConnections)
		{
			LayoutConnection newConnection = ScriptableObject.CreateInstance<LayoutConnection>();
			newConnection.Source = this;
			newConnection.Target = other;
			
			m_connections.Add(newConnection);	
			other.m_connections.Add(newConnection);
		}
		else
		{
			Debug.Log("Maximum LayoutNode connections already made");
		}
	}
	
	public void RemoveConnection(LayoutNode other)
	{
		LayoutConnection foundConnection = null;
		foreach(var connection in m_connections)
		{
			if(connection.Source == other || connection.Target == other)
			{
				foundConnection = connection;
				break;
			}
		}
		if(foundConnection != null)
		{
			m_connections.Remove(foundConnection);	
			other.m_connections.Remove(foundConnection);	
		}
	}
	
	public void RemoveConnection(LayoutConnection connection)
	{
		connection.Source.m_connections.Remove(connection);
		connection.Target.m_connections.Remove(connection);
	}
	
	public void RemoveAllConnections()
	{
		while(m_connections.Count > 0)
		{
			RemoveConnection(m_connections[0]);	
		}
	}
	
	public List<LayoutConnection> ConnectedNodes
	{
		get { return m_connections; }	
	}
	
	public virtual List<GameObject> BuildObject()
	{
		Debug.Log("Building base-class");
		return null;
	}
	
	public virtual bool OnGUIInternal(EditType editType, int id, bool selectedNode)
	{
		switch(editType)
		{
			case EditType.Move: 
			{
				Vector3	newPos =  Handles.Slider2D(id, m_position, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CubeCap, new Vector2(0.5f, 0.5f));
			
				if(newPos.x != m_position.x || newPos.y != m_position.y)
				{
					m_position = newPos;
				}
			
				bool selected = GUIUtility.hotControl == id;
				if(selected && ConnectedNodes.Count > 0 && SelectedConnection == null)
				{
					SelectedConnection = ConnectedNodes[0];
				}
			
				return selected;
			}
			case EditType.Link:
			{
				return Handles.Button(m_position, Quaternion.identity, 0.2f, 0.2f, Handles.CubeCap);
			}
		}
		
		return false;
	}
	
	public virtual void OnInspectorGUI()
	{
			
		GUILayout.Label("Position");
		m_position.x = EditorGUILayout.FloatField("x", m_position.x);
		m_position.y = EditorGUILayout.FloatField("y", m_position.y);
		
	
		EditorGUILayout.BeginVertical();
		
		m_showConnections = EditorGUILayout.Foldout(m_showConnections, "Connections");
		
		if(m_showConnections)
		{
			string[] connections = new string[ConnectedNodes.Count];
	
			int selectedIndex = 0;
			for(int i = 0; i < ConnectedNodes.Count; i++)
			{
				connections[i] = "connection";
				if(ConnectedNodes[i] == SelectedConnection)
				{
					selectedIndex = i;	
				}
			}
		
			int newSelection = GUILayout.SelectionGrid(selectedIndex, connections, 1);
		
			if(newSelection != selectedIndex)
			{
				SelectedConnection = ConnectedNodes[newSelection];
				EditorUtility.SetDirty(this);
			}
			
			GUILayout.Label("Selected Connection");
			if(SelectedConnection != null)
			{
				SelectedConnection.ConnectionType = (LayoutConnection.ConnectionTypes)EditorGUILayout.EnumPopup("Connection Type", SelectedConnection.ConnectionType);
				
				if(SelectedConnection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier)
				{
					SelectedConnection.BezierSections = EditorGUILayout.IntField("Bezier Sections", SelectedConnection.BezierSections);
				}
				
				if(GUILayout.Button("Delete"))
				{
					RemoveConnection(SelectedConnection);
					if(ConnectedNodes.Count > 0)
					{
						SelectedConnection = ConnectedNodes[0];
						EditorUtility.SetDirty(this);
					}
				}
			}
		}
	
		EditorGUILayout.EndVertical();
	}
	
	public Vector2 GetEndPoints(LayoutConnection connection)
	{
		Vector2 endPoints = new Vector2(0.0f, 0.0f);
		
		if(!m_connections.Contains(connection))
		{
			Debug.Log("Connection-angle requested from a node that doesn't contain connection");
			return endPoints;
		}
		
		foreach(var other in m_connections)
		{
			if(other != connection)
			{
				LayoutNode otherNode = connection.Source == this ? connection.Target : connection.Source;
			
				Vector2 directionToOther = otherNode.m_position - m_position;
				float rotation = Mathf.Atan2(directionToOther.x, directionToOther.y);
				
				
				Vector3 otherSource = other.Source == this ? other.Source.m_position : other.Target.m_position;
				Vector3 otherTarget = other.Source == this ? other.Target.m_position : other.Source.m_position;
				Vector3 otherDirection = otherTarget - otherSource;
				
				float angle = Mathf.Atan2(otherDirection.x, otherDirection.y);
				
				float wallSize = 0.2f;
				
				float halfAngle = (angle + rotation) / 2.0f;
				float localHalfAngle = halfAngle- rotation;
				localHalfAngle = (Mathf.PI / 2.0f) - localHalfAngle;
				
				
									
				float yChangeLeft = Mathf.Sin(localHalfAngle);
				
				float hyp = wallSize / Mathf.Cos(localHalfAngle );
				Debug.Log("Arc length: " + hyp + " | " + wallSize);
				yChangeLeft *= hyp;
				
				float yChangeRight = -yChangeLeft;
				endPoints.x = yChangeLeft;
				endPoints.y = yChangeRight;

			}
		}
		
		return endPoints;
	}
	
	public LayoutConnection SelectedConnection
	{
		get; set;	
	}
	
	[SerializeField]
	public Vector2 m_position;
	
	[SerializeField]
	protected int m_maxConnections = 10;
	
	[SerializeField]
	protected List<LayoutConnection> m_connections = new List<LayoutConnection>();
	
	[SerializeField]
	private bool m_showConnections = true;
}
