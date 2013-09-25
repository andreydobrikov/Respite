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
		
		Handles.color = new Color(0.0f, 0.0f, 0.6f, 1.0f);
		for(int i = 0; i < spline.m_beziers.Length + 1; ++i)
		{
			Vector2 normal;
			
			if(i == spline.m_beziers.Length)
			{
				normal = Bezier.GetBezierNormal(1.0f, spline.m_beziers[i - 1].m_v0, spline.m_beziers[i - 1].m_t0, spline.m_beziers[i - 1].m_v1, spline.m_beziers[i - 1].m_t1);	
				
				// Spunk these into 3D
				Vector3 normal3D = new Vector3(normal.x, 0.0f, normal.y);
				Vector3 v1 = new Vector3(spline.m_beziers[i - 1].m_v1.x, 0.0f, spline.m_beziers[i - 1].m_v1.y);
				
				Handles.DrawLine(v1, v1 + (normal3D * spline.m_widthModifiers[i]));
				Handles.DrawLine(v1, v1 - (normal3D * spline.m_widthModifiers[i]));
			}
			else
			{
				normal = Bezier.GetBezierNormal(0.0f, spline.m_beziers[i].m_v0, spline.m_beziers[i].m_t0, spline.m_beziers[i].m_v1, spline.m_beziers[i].m_t1);	
				
				Vector3 normal3D = new Vector3(normal.x, 0.0f, normal.y);
				Vector3 v0 = new Vector3(spline.m_beziers[i].m_v0.x, 0.0f, spline.m_beziers[i].m_v0.y);
				
				Handles.DrawLine(v0, v0 + (normal3D * spline.m_widthModifiers[i]));
				Handles.DrawLine(v0, v0 - (normal3D * spline.m_widthModifiers[i]));
			}
			 
			
			
		}
	}
	
	public static void DrawInspectorGUI(Spline spline)
	{
		int controlPointCount = EditorGUILayout.IntField("Control Points", spline.m_beziers.Length);
		
		if(controlPointCount > 2)
		{
			if(controlPointCount != spline.m_beziers.Length)
			{
				spline.ResizeSpline(controlPointCount);
			}
		}
		
		EditorGUILayout.BeginVertical((GUIStyle)("Box"));
		
		spline.showModifiers = EditorGUILayout.Foldout(spline.showModifiers, "Width Modifiers");
		
		if(spline.showModifiers)
		{
			for(int i = 0; i < spline.m_beziers.Length + 1; ++i)
			{
				spline.m_widthModifiers[i] = EditorGUILayout.FloatField("Control Point " + i, spline.m_widthModifiers[i]);
			}
		}
		
		EditorGUILayout.EndVertical();
		
		
		
	}
}
