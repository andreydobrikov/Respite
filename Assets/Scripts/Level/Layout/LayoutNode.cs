using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

[Serializable]
public class LayoutNode : ScriptableObject
{
	public enum EditType
	{
		Move,
		Link
	}
	
	public void SetOwner(Level owner)
	{
		m_owner = owner;	
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
	
	
	
	public virtual List<GameObject> BuildObject()
	{
		Debug.Log("Building base-class");
		return null;
	}
	
	#if UNITY_EDITOR
	public virtual bool OnGUIInternal(EditType editType, int id, bool selectedNode)
	{
		switch(editType)
		{
			case EditType.Move: 
			{
				Vector3	newPos =  Handles.Slider2D(id, LocalPosition, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CubeCap, new Vector2(0.5f, 0.5f));
			
				if(newPos.x != LocalPosition.x || newPos.y != LocalPosition.y)
				{
					LocalPosition = newPos;
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
				return Handles.Button(LocalPosition, Quaternion.identity, 0.2f, 0.2f, Handles.CubeCap);
			}
		}
		
		return false;
	}
	
	public virtual void OnInspectorGUI()
	{
		GUILayout.Label("Position");
		
		Vector2 newPos;
		newPos.x = EditorGUILayout.FloatField("x", LocalPosition.x);
		newPos.y = EditorGUILayout.FloatField("y", LocalPosition.y);
		
		LocalPosition = newPos;
	
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
#endif
	
	public Vector2 GetEndPoints(LayoutConnection connection)
	{
		Vector2 endPoints = new Vector2(0.0f, 0.0f);
		
		if(!m_connections.Contains(connection))
		{
			Debug.Log("Connection-angle requested from a node that doesn't contain connection");
			return endPoints;
		}
		
		// Find the node at the other end of the connection
		LayoutNode otherNode = connection.Source == this ? connection.Target : connection.Source;
		
		// Determine the direction of the connection from this node
		Vector2 directionToTarget = otherNode.LocalPosition - LocalPosition;
		
		// And it's rotation relative to up
		float rotation = Mathf.Atan2(directionToTarget.x, directionToTarget.y);
		
		// Let's sort these connections on their rotation
		int index = m_connections.IndexOf(connection);
		
		int leftIndex = index - 1;
		int rightIndex = index + 1;
	
		if(leftIndex < 0) leftIndex = (m_connections.Count) + leftIndex;
		rightIndex = rightIndex % m_connections.Count;
		
		float leftAdjust = 0.0f;
		float rightAdjust = 0.0f;
		
		// Left angle
		{
			LayoutConnection leftConnection = m_connections[rightIndex];
			Vector3 otherSource = leftConnection.Source == this ? leftConnection.Source.LocalPosition : leftConnection.Target.LocalPosition;
			Vector3 otherTarget = leftConnection.Source == this ? leftConnection.Target.LocalPosition : leftConnection.Source.LocalPosition;
			Vector3 otherDirection = otherTarget - otherSource;
			
			float angle = Mathf.Atan2(otherDirection.x, otherDirection.y);
			
			float halfAngle = (angle + rotation) / 2.0f;
			float localHalfAngle = halfAngle- rotation;
			localHalfAngle = (Mathf.PI / 2.0f) - localHalfAngle;
			
			float yChangeLeft = Mathf.Sin(localHalfAngle);
			
			float hyp = m_wallWidth / Mathf.Cos(localHalfAngle );
			yChangeLeft *= hyp;
			leftAdjust = yChangeLeft;
				
		}
		
		// Right angle
		{
			LayoutConnection leftConnection = m_connections[leftIndex];
			Vector3 otherSource = leftConnection.Source == this ? leftConnection.Source.LocalPosition : leftConnection.Target.LocalPosition;
			Vector3 otherTarget = leftConnection.Source == this ? leftConnection.Target.LocalPosition : leftConnection.Source.LocalPosition;
			Vector3 otherDirection = otherTarget - otherSource;
			
			float angle = Mathf.Atan2(otherDirection.x, otherDirection.y);
			
			float halfAngle = (angle + rotation) / 2.0f;
			float localHalfAngle = halfAngle- rotation;
			localHalfAngle = (Mathf.PI / 2.0f) - localHalfAngle;
			
			float yChangeLeft = Mathf.Sin(localHalfAngle);
			
			float hyp = m_wallWidth / Mathf.Cos(localHalfAngle );
			yChangeLeft *= hyp;
			rightAdjust = -yChangeLeft;
				
		}
		
		foreach(var other in m_connections)
		{
			if(other != connection)
			{
				endPoints.x = leftAdjust;
				endPoints.y = rightAdjust;
			}
		}
		
		return endPoints;
	}
	
	#region Properties
	
	public List<LayoutConnection> ConnectedNodes
	{
		get { return m_connections; }	
	}
	
	public LayoutConnection SelectedConnection
	{
		get; set;	
	}
	
	public Vector2 LocalPosition
	{
		get 
		{ 
			Vector2 ownerPos = m_owner.transform.position;
			return m_worldPosition + ownerPos;
		}
		
		set
		{
			Vector2 ownerPos = m_owner.transform.position;	
			m_worldPosition = value - ownerPos;
		}
	}
	
	#endregion
	
	#region Fields
	
	[SerializeField]
	public Level m_owner;
	
	[SerializeField]
	public Vector2 m_worldPosition;
	
	[SerializeField]
	public float m_wallWidth = 0.2f;
	
	[SerializeField]
	protected int m_maxConnections = 10;
	
	[SerializeField]
	protected List<LayoutConnection> m_connections = new List<LayoutConnection>();
	
	[SerializeField]
	private bool m_showConnections = true;
	
	#endregion
}
