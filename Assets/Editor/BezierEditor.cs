using UnityEditor;
using UnityEngine;
using System.Collections;

public class BezierEditor
{
	// Fucking inscrutable since I twiddled the world
	
	public static void DrawEditor(Bezier bezier, bool pathOnly)
	{
		Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);
		
		Vector3 v03D = new Vector3(bezier.m_v0.x, 0.0f, bezier.m_v0.y);
		Vector3 v13D = new Vector3(bezier.m_v1.x, 0.0f, bezier.m_v1.y);
		Vector3 t03D = new Vector3(bezier.m_t0.x, 0.0f, bezier.m_t0.y);
		Vector3 t13D = new Vector3(bezier.m_t1.x, 0.0f, bezier.m_t1.y);
		
		float handleSize = HandleUtility.GetHandleSize(v03D + offset) / 8.0f;
		
		Handles.DrawBezier(v03D + offset, v13D + offset, t03D + offset, t13D + offset, Color.red, null, 2.0f);	
		
		if(pathOnly)
		{
			return;	
		}
		
		Handles.color = Color.blue;
		v03D = Handles.Slider2D(v03D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		v13D = Handles.Slider2D(v13D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		Handles.color = new Color(0.0f, 0.5f, 0.0f, 1.0f);
		t03D = Handles.Slider2D(t03D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		t13D = Handles.Slider2D(t13D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		bezier.m_v0 = new Vector2(v03D.x, v03D.z);
		bezier.m_v1 = new Vector2(v13D.x, v13D.z);
		bezier.m_t0 = new Vector2(t03D.x, t03D.z);
		bezier.m_t1 = new Vector2(t13D.x, t13D.z);
		
		Handles.DrawLine(v03D, t03D);
		Handles.DrawLine(v13D, t13D);
	}
	
	public static void DrawEditor(Bezier bezier, Bezier before, Bezier after, bool pathOnly)
	{
		Vector3 offset = new Vector3(0.0f, 1.0f, 0.0f);
		
		Vector3 v03D = new Vector3(bezier.m_v0.x, 0.0f, bezier.m_v0.y);
		Vector3 v13D = new Vector3(bezier.m_v1.x, 0.0f, bezier.m_v1.y);
		Vector3 t03D = new Vector3(bezier.m_t0.x, 0.0f, bezier.m_t0.y);
		Vector3 t13D = new Vector3(bezier.m_t1.x, 0.0f, bezier.m_t1.y);
		
		float handleSize = HandleUtility.GetHandleSize((Vector3)bezier.m_v0 + offset) / 8.0f;
		
		Handles.DrawBezier(v03D + offset, v13D + offset, t03D + offset, t13D + offset, Color.red, null, 2.0f);	
		
		if(pathOnly)
		{
			return;	
		}
		
		Handles.color = Color.blue;
		
		// Find the offset of the tangent from the control-point before moving the control-point.
		Vector3 t0Diff = t03D - v03D;
		Vector3 t1Diff = t13D - v13D;
		
		v03D = Handles.Slider2D(v03D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		v13D = Handles.Slider2D(v13D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		bezier.m_v0 = new Vector2(v03D.x, v03D.z);
		bezier.m_v1 = new Vector2(v13D.x, v13D.z);
		
		// Shift the tangents to their new relative position
		// TODO: Assign back to the object here!
		t03D = v03D + t0Diff;
		t13D = v13D + t1Diff;
		
		bezier.m_t0 = new Vector2(t03D.x, t03D.z);
		bezier.m_t1 = new Vector2(t13D.x, t13D.z);
		
		Handles.color = new Color(0.0f, 0.5f, 0.0f, 1.0f);
		t03D = Handles.Slider2D(t03D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		t13D = Handles.Slider2D(t13D, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		
		bezier.m_t0 = new Vector2(t03D.x, t03D.z);
		bezier.m_t1 = new Vector2(t13D.x, t13D.z);
		
		Handles.DrawLine(v03D, t03D);
		Handles.DrawLine(v13D, t13D);
		
		t0Diff = t03D - v03D;
		t1Diff = t13D - v13D;
		
		if(after != null)
		{
			after.m_v0 =  new Vector2(v13D.x, v13D.z);
			
			after.m_t0 = new Vector2((v13D - t1Diff).x, (v13D - t1Diff).z);
		}
		
		if(before != null)
		{
			before.m_v1 = new Vector2(v03D.x, v03D.z);
			
			before.m_t1 = new Vector2((v03D - t0Diff).x, (v03D - t0Diff).z);
		}
		
	}
}
