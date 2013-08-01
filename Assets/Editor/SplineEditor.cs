using UnityEditor;
using UnityEngine;
using System.Collections;

public class SplineEditor
{
	
	public static void DrawEditor(Spline spline, bool pathOnly)
	{
		
		for(int i = 0; i < spline.m_beziers.Length; ++i)
		{
			BezierEditor.DrawEditor(spline.m_beziers[i], i > 0 ? spline.m_beziers[i-1] : null, i < (spline.m_beziers.Length - 1) ? spline.m_beziers[i+1] : null, pathOnly);	
		}
	}
	
	public static void DrawInspectorGUI(Spline spline)
	{
		int controlPointCount = EditorGUILayout.IntField("Control Points", spline.m_beziers.Length);
		
		if(controlPointCount > 4)
		{
			if(controlPointCount != spline.m_beziers.Length)
			{
				spline.ResizeSpline(controlPointCount);
			}
		}
		
	}
}
