using UnityEditor;
using UnityEngine;
using System.Collections;

public class BezierEditor
{
	
	public static void DrawEditor(Bezier bezier, bool pathOnly)
	{
		Vector3 offset = new Vector3(0.0f, 0.0f, -1.0f);
		float handleSize = HandleUtility.GetHandleSize((Vector3)bezier.m_v0 + offset) / 8.0f;
		
		Handles.DrawBezier((Vector3)bezier.m_v0 + offset, (Vector3)bezier.m_v1 + offset, (Vector3)bezier.m_t0 + offset, (Vector3)bezier.m_t1 + offset, Color.red, null, 2.0f);	
		
		if(pathOnly)
		{
			return;	
		}
		
		Handles.color = Color.blue;
		bezier.m_v0 = Handles.Slider2D(bezier.m_v0, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		bezier.m_v1 = Handles.Slider2D(bezier.m_v1, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		Handles.color = new Color(0.0f, 0.5f, 0.0f, 1.0f);
		bezier.m_t0 = Handles.Slider2D(bezier.m_t0, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		bezier.m_t1 = Handles.Slider2D(bezier.m_t1, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		Handles.DrawLine(bezier.m_v0, bezier.m_t0);
		Handles.DrawLine(bezier.m_v1, bezier.m_t1);
	}
	
	public static void DrawEditor(Bezier bezier, Bezier before, Bezier after, bool pathOnly)
	{
		Vector3 offset = new Vector3(0.0f, 0.0f, -1.0f);
		float handleSize = HandleUtility.GetHandleSize((Vector3)bezier.m_v0 + offset) / 8.0f;
		
		Handles.DrawBezier((Vector3)bezier.m_v0 + offset, (Vector3)bezier.m_v1 + offset, (Vector3)bezier.m_t0 + offset, (Vector3)bezier.m_t1 + offset, Color.red, null, 2.0f);	
		
		if(pathOnly)
		{
			return;	
		}
		
		Handles.color = Color.blue;
		
		// Find the offset of the tangent from the control-point before moving the control-point.
		Vector2 t0Diff = bezier.m_t0 - bezier.m_v0;
		Vector2 t1Diff = bezier.m_t1 - bezier.m_v1;
		
		bezier.m_v0 = Handles.Slider2D(bezier.m_v0, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		bezier.m_v1 = Handles.Slider2D(bezier.m_v1, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		// Shift the tangents to their new relative position
		bezier.m_t0 = bezier.m_v0 + t0Diff;
		bezier.m_t1 = bezier.m_v1 + t1Diff;
		
		Handles.color = new Color(0.0f, 0.5f, 0.0f, 1.0f);
		bezier.m_t0 = Handles.Slider2D(bezier.m_t0, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		bezier.m_t1 = Handles.Slider2D(bezier.m_t1, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		
		Handles.DrawLine(bezier.m_v0, bezier.m_t0);
		Handles.DrawLine(bezier.m_v1, bezier.m_t1);
		
		t0Diff = bezier.m_t0 - bezier.m_v0;
		t1Diff = bezier.m_t1 - bezier.m_v1;
		
		if(after != null)
		{
			after.m_v0 = bezier.m_v1;
			
			after.m_t0 = bezier.m_v1 - t1Diff;
		}
		
		if(before != null)
		{
			before.m_v1 = bezier.m_v0;	
			
			before.m_t1 = bezier.m_v0 - t0Diff;
		}
		
	}
}
