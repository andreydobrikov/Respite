/// <summary>
/// Game flow wrapper.
/// 
/// Just a damned editor class. Don't have a crap-attack.
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameFlowWrapper))] 
public class GameFlowWrapperEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		GameFlowWrapper wrapper = (GameFlowWrapper)target;
		
		EditorGUILayout.BeginVertical((GUIStyle)("Box"));
		
		wrapper.ShowFoldout = EditorGUILayout.Foldout(wrapper.ShowFoldout, "Parameters");

		
		if(wrapper.ShowFoldout)
		{
			wrapper.GameDuration = EditorGUILayout.FloatField("Game Duration (seconds)", wrapper.GameDuration);
			wrapper.SaveFadeDuration = EditorGUILayout.FloatField("Save Fade Duration", wrapper.SaveFadeDuration);
		}
		
		EditorGUILayout.EndVertical();
	}
}
