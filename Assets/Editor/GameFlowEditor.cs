using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GameFlowEditor : EditorWindow 
{
	[MenuItem("Optimism/Startup", true)]
	public static bool ShowWindowValidate()
	{
		GameFlowWrapper wrapperObject = GameObject.FindObjectOfType(typeof(GameFlowWrapper)) as GameFlowWrapper;
		return wrapperObject != null;
	}
	
	[MenuItem("Optimism/Startup", false)]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(GameFlowEditor));
	}

	void OnGUI()
	{
		GameFlowWrapper wrapperObject = GameObject.FindObjectOfType(typeof(GameFlowWrapper)) as GameFlowWrapper;
		
		GUILayout.Label ("Agent Instantiation Objects", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginVertical();
		
		List<GameObject> newItems = new List<GameObject>();
		 
		foreach(GameObject item in wrapperObject.AgentStartupItems)
		{
			EditorGUILayout.BeginHorizontal();
			GameObject newObject = EditorGUILayout.ObjectField(item, typeof(GameObject), false) as GameObject;
			EditorGUILayout.EndHorizontal();
			
			if(newObject != null)
			{
				newItems.Add(newObject);	
			}
		}
		
		EditorGUILayout.BeginHorizontal();
		GameObject blankObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		
		if(blankObject != null)
		{
			newItems.Add(blankObject);	
		}
		
		wrapperObject.AgentStartupItems = newItems;
		
		// Admin
		
		
		GUILayout.Label ("Admin Instantiation Objects", EditorStyles.boldLabel);
		
		newItems = new List<GameObject>();
		
		foreach(GameObject item in wrapperObject.AdminStartupItems)
		{
			EditorGUILayout.BeginHorizontal();
			GameObject newObject = EditorGUILayout.ObjectField(item, typeof(GameObject), false) as GameObject;
			EditorGUILayout.EndHorizontal();
			
			if(newObject != null)
			{
				newItems.Add(newObject);	
			}
		}
		
		EditorGUILayout.BeginHorizontal();
		blankObject = EditorGUILayout.ObjectField(null, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		
		if(blankObject != null)
		{
			newItems.Add(blankObject);	
		}
		
		wrapperObject.AdminStartupItems = newItems;
		
		EditorGUILayout.EndVertical();
		
		GUILayout.Button("Add type");
		
		
	}
}
