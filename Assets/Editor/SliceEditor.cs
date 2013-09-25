///////////////////////////////////////////////////////////
// 
// SliceEditor.cs
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


public class SliceEditor : Editor 
{

	void OnSceneGUI()
	{
		Island island = (Island)target;
		
		for(int i = 0; i < island.m_triangles.Count; i++)
		{
			MeshSlice.Triangle tri = island.m_triangles[i];
			
			Vector3 v0 = new Vector3(tri.p0.x, 1.0f, tri.p0.y);	
			Vector3 v1 = new Vector3(tri.p1.x, 1.0f, tri.p1.y);
			Vector3 v2 = new Vector3(tri.p2.x, 1.0f, tri.p2.y);
			
			Vector3 v3 = new Vector3(island.sliceStart.x, 1.0f, island.sliceStart.y);
			Vector3 v4 = new Vector3(island.sliceEnd.x, 1.0f, island.sliceEnd.y);
			
			
			float handleSize = HandleUtility.GetHandleSize((Vector3)v0 ) / 8.0f;
			
			v0 = Handles.Slider2D(v0, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
			v1 = Handles.Slider2D(v1, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
			v2 = Handles.Slider2D(v2, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
			
			v3 = Handles.Slider2D(v3, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
			v4 = Handles.Slider2D(v4, Vector3.up, Vector3.right, Vector3.forward, handleSize, Handles.SphereCap, new Vector2(0.1f, 0.1f));
			
			
			Handles.DrawLine(v0, v1);
			Handles.DrawLine(v1, v2);
			Handles.DrawLine(v2, v0);
			
			
			Handles.DrawLine(v3, v4);
			
			tri.p0 = new Vector2(v0.x, v0.z);
			tri.p1 = new Vector2(v1.x, v1.z);
			tri.p2 = new Vector2(v2.x, v2.z);
			
			island.m_triangles[i] = tri;
			
			island.sliceStart = new Vector2(v3.x, v3.z);
			island.sliceEnd = new Vector2(v4.x, v4.z);
		}
	}
	
	public override void OnInspectorGUI()
	{
		Island island = (Island)target;
		
		island.IslandSourceMesh = EditorGUILayout.ObjectField(island.IslandSourceMesh, typeof(Mesh), true) as Mesh;
		
		island.SectionsX = EditorGUILayout.IntField("Slices X", island.SectionsX);
		island.SectionsY = EditorGUILayout.IntField("Slices Y", island.SectionsY);
		
		if(GUILayout.Button("Slice Mesh"))
		{
			if(island.IslandSourceMesh != null)
			{
				Mesh[,] meshes = MeshSlice.Slice(island.IslandSourceMesh, island.SectionsX, island.SectionsY, true, true);
					
				GameObject meshesObject = GameObjectHelper.FindChild(island.gameObject, "meshes", true);
				
				foreach(var mesh in meshes)
				{
					mesh.Optimize();
					GameObject newObject = new GameObject("blah");
					newObject.transform.parent = meshesObject.transform;
					newObject.transform.localPosition = Vector3.zero;
					newObject.AddComponent<MeshRenderer>();
					
					MeshFilter filter = newObject.AddComponent<MeshFilter>();
					filter.mesh = mesh;
				}
			}
		}
		
		if(GUILayout.Button("Reset"))
		{
			island.m_triangles.Clear();
			
			
			MeshSlice.Triangle tri = new MeshSlice.Triangle();
			tri.p0 = new Vector2(-1.0f, -1.0f);
			tri.p1 = new Vector2(1.0f, -1.0f);
			tri.p2 = new Vector2(0.0f, 1.0f);
			island.m_triangles.Add(tri);
		}
		
		if(GUILayout.Button("Slice"))
		{
			List<MeshSlice.Triangle> tris = island.m_triangles;
			
			List<MeshSlice.Triangle> outTris = new List<MeshSlice.Triangle>();
			
			Vector2 v0, v1;
			
			foreach(var tri in tris)
			{
				if(MathsHelper.LineTriIntersect(island.sliceStart, island.sliceEnd, tri.p0, tri.p1, tri.p2, out v0, out v1))
				{
					outTris.AddRange(MeshSlice.SliceTri(tri, island.sliceStart, island.sliceEnd));	
				}
				else
				{
					outTris.Add(tri);	
				}
			}
			
			island.m_triangles = outTris;
			
		}
	}
}
