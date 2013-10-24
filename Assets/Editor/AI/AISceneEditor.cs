///////////////////////////////////////////////////////////
// 
// AISceneEditor.cs
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

[CustomEditor(typeof(AI))] 
public class AISceneEditor : Editor 
{
	void OnSceneGUI()
	{
		Handles.DrawLine(Vector3.zero, Vector3.one);
		
		AI activeAI = (AI)target;
		
		if(activeAI.SelectedState < activeAI.States.Count && activeAI.SelectedState >= 0)
		{
			AIState currentState = activeAI.States[activeAI.SelectedState];
			
			foreach(var behaviour in currentState.Behaviours)
			{
				behaviour.OnSceneGUI();
			}
			
			EditorUtility.SetDirty(activeAI);
			
			Repaint();
		}
		
	}
}
