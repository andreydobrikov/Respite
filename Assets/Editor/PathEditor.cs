using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Path))] 
public class PathEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Path path = (Path)target;
		
		bool pathOnly = EditorGUILayout.Toggle("Draw Path Only", path.m_drawPathOnly);
		
		if(pathOnly != path.m_drawPathOnly)
		{
			path.m_drawPathOnly = pathOnly;
			EditorUtility.SetDirty(path);
		}
		
		SplineEditor.DrawInspectorGUI(path.m_spline);
		
	}
	
	public void OnSceneGUI()
	{
		Path path = (Path)target;
		
		SplineEditor.DrawEditor(path.m_spline, path.m_drawPathOnly);		
	}
}
