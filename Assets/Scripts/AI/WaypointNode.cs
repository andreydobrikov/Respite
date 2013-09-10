///////////////////////////////////////////////////////////
// 
// WaypointNode.cs
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

public class WaypointNode : ScriptableObject
{
	public int ID = -1;
	
	public int sequenceIndex = -1;
	
	public Vector3 position = Vector2.zero;
	
	public List<WaypointNode> m_connections = new List<WaypointNode>();
	
	public float HoldTime = 0.0f;
}