///////////////////////////////////////////////////////////
// 
// IntersectionTestEditor.cs
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

[CustomEditor(typeof(IntersectionTester))]
public class IntersectionTestEditor : Editor 
{

	void OnSceneGUI()
	{
		IntersectionTester tester = (IntersectionTester)target;
		
		float handleSize = HandleUtility.GetHandleSize((Vector3)tester.v0 ) / 8.0f;
		
		tester.v0 = Handles.Slider2D(tester.v0, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		tester.v1 = Handles.Slider2D(tester.v1, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		tester.v2 = Handles.Slider2D(tester.v2, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		tester.v3 = Handles.Slider2D(tester.v3, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		tester.v4 = Handles.Slider2D(tester.v4, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
		
		Vector2 altV0 = new Vector2(tester.v0.x, tester.v0.z);
		Vector2 altV1 = new Vector2(tester.v1.x, tester.v1.z);
		Vector2 altV2 = new Vector2(tester.v2.x, tester.v2.z);
		Vector2 altV3 = new Vector2(tester.v3.x, tester.v3.z);
		Vector2 altV4 = new Vector2(tester.v4.x, tester.v4.z);
		
		Vector2 i0, i1;
		
		bool intersection = MathsHelper.LineTriIntersect(altV0, altV1, altV2, altV3, altV4, out i0, out i1);
		
		intersection |= MathsHelper.LineInTri(altV0, altV1, altV2, altV3, altV4);

		Handles.color = intersection ? Color.red : Color.green;
		
		Handles.DrawLine(tester.v0, tester.v1);
		
		Handles.DrawLine(tester.v2, tester.v3);
		Handles.DrawLine(tester.v4, tester.v3);
		Handles.DrawLine(tester.v4, tester.v2);
	}
}
