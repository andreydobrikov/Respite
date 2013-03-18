using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameFlowWrapper))]
public class GameFlowWrapperEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		GameFlowWrapper myTarget = (GameFlowWrapper) target;
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.LabelField("Level Object");
		
		var newLevelObject = EditorGUILayout.ObjectField(myTarget.LevelObject, typeof(GameObject), true, null) as GameObject;
		myTarget.LevelObject = newLevelObject;
		
		EditorGUILayout.EndHorizontal();
		
	}
}
