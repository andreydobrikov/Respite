///////////////////////////////////////////////////////////
// 
// StatsTracker.cs
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

public class StatsTracker : MonoBehaviour 
{
	void OnLevelWasLoaded()
	{
		Debug.Log("Level loaded");
		OccludedMesh.m_meshes.Clear();
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 180, 1000));
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		GUILayout.Label("Occluded Meshes: " + OccludedMesh.m_activeMeshes);
		
		foreach(var mesh in OccludedMesh.m_meshes)
		{
			GUILayout.Label(" - " + mesh.name);	
		}
		
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
	}
}
