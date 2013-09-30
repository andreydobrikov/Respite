///////////////////////////////////////////////////////////
// 
// HeightPainterEditor.cs
//
// What it does: Custom editor for the HeightPainter that allows setting of thresholds and brushes.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(HeightPainter))]
public class HeightPainterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HeightPainter painter = (HeightPainter)target;	
		
		painter.m_brush.ShowInspectorGUI();
		
		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 5));
		
		painter.m_heightThreshold 	= EditorGUILayout.FloatField("Height Threshold", painter.m_heightThreshold);
		painter.m_heightBlend 		= EditorGUILayout.FloatField("Height Blend", painter.m_heightBlend);
		
	}
}
