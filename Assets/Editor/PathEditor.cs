using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(Path))] 
public class PathEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Path path = (Path)target;
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		GUILayout.Label("Spline Settings");
		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 20));
		
		bool pathOnly = EditorGUILayout.Toggle("Draw Path Only", path.m_drawPathOnly);
		
		if(pathOnly != path.m_drawPathOnly)
		{
			path.m_drawPathOnly = pathOnly;
			EditorUtility.SetDirty(path);
		}
		
		SplineEditor.DrawInspectorGUI(path.m_spline);
		
		GUILayout.EndVertical();
		
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		path.m_showGeneratorFoldout = EditorGUILayout.Foldout(path.m_showGeneratorFoldout, "Generator");
		
		if(path.m_showGeneratorFoldout)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			path.m_meshes = EditorGUILayout.Toggle("Generate Meshes", path.m_meshes);
			path.m_colliders = EditorGUILayout.Toggle("Generate Colliders", path.m_colliders);
			
			GUILayout.EndVertical();
			
			GUI.enabled = path.m_colliders;
			
			// Colliders
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			path.m_showCollidersFoldout = EditorGUILayout.Foldout(path.m_showCollidersFoldout, "Collider Settings");
			
			if(path.m_showCollidersFoldout)
			{
				GUILayout.BeginVertical((GUIStyle)("Box"));
				
				path.m_colliderSectionCount = EditorGUILayout.IntField("Collider Count", path.m_colliderSectionCount);	
				path.m_colliderWidthMultiplier = EditorGUILayout.FloatField("Collider Width Modifier", path.m_colliderWidthMultiplier);
				
				GUILayout.EndVertical();
			}
			
			GUILayout.EndVertical();
			
			// Meshes
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			path.m_showMeshesFoldout = EditorGUILayout.Foldout(path.m_showMeshesFoldout, "Mesh Settings");
			
			if(path.m_showMeshesFoldout)
			{
				GUILayout.BeginVertical((GUIStyle)("Box"));
				path.m_meshMaterial 	= EditorGUILayout.ObjectField(path.m_meshMaterial, typeof(Material), false) as Material;
				path.m_meshLayer		= EditorGUILayout.LayerField("Mesh Layer", path.m_meshLayer);
				path.m_meshDepth		= EditorGUILayout.FloatField("Mesh Z", path.m_meshDepth);
				path.m_meshWidth		= EditorGUILayout.FloatField("Mesh Segment Radius", path.m_meshWidth);
				path.m_uv0Multiplier	= EditorGUILayout.FloatField("UV0 Multiplier", path.m_uv0Multiplier);
				path.m_uv1Multiplier	= EditorGUILayout.FloatField("UV1 Multiplier", path.m_uv1Multiplier);
				int newSegmentCount 	= EditorGUILayout.IntField("Mesh Segment Count", path.m_meshSegmentCount);	
				
				const int minSegments = 2;
				const int maxSegments = 1000;
				
				if(newSegmentCount != path.m_meshSegmentCount && newSegmentCount > minSegments && newSegmentCount < maxSegments)
				{
					path.m_meshSegmentCount	= newSegmentCount;
				}
				GUILayout.EndVertical();
			}
			
			GUILayout.EndVertical();
			
			GUI.enabled = path.m_colliders || path.m_meshes;
			
			if(GUILayout.Button("Rebuild Path"))
			{
				GameObject meshesObject = GameObjectHelper.FindChild(path.gameObject, "meshes", true);
				GameObject collidersObject = GameObjectHelper.FindChild(path.gameObject, "colliders", true);
				
				int count = 0;
				foreach(var bezier in path.m_spline.m_beziers)
				{
					float startWidth = path.m_spline.m_widthModifiers[count];
					float endWidth = path.m_spline.m_widthModifiers[count + 1];
					
					Mesh pathMesh = Bezier.GetBezierMesh(bezier.m_v0, bezier.m_t0, bezier.m_v1, bezier.m_t1, path.m_meshSegmentCount, false, path.m_meshWidth, startWidth, endWidth, path.m_uv0Multiplier, path.m_uv1Multiplier);
					
					if(path.m_meshes)
					{
						GameObject newObject = new GameObject("Segment (" + bezier.m_v0.x + ", " + bezier.m_v0.y + ")");
						newObject.layer = path.m_meshLayer;
						
						newObject.transform.parent 			= meshesObject.transform;
					//	newObject.transform.localPosition 	= new Vector3(0.0f, 0.0f, 0.0f);
						newObject.transform.rotation		= Quaternion.identity;
						newObject.transform.position 		= new Vector3(0.0f, 0.0f, path.m_meshDepth);
						
						MeshFilter filter 				= newObject.AddComponent<MeshFilter>();
						MeshRenderer renderer 			= newObject.AddComponent<MeshRenderer>();
						
						renderer.sharedMaterial				= path.m_meshMaterial;
						
						
						
						filter.mesh = pathMesh;
						
					}
					
					if(path.m_colliders)
					{
						Mesh colliderMesh = Bezier.GetBezierMesh(bezier.m_v0, bezier.m_t0, bezier.m_v1, bezier.m_t1, path.m_meshSegmentCount, false, path.m_meshWidth * path.m_colliderWidthMultiplier, startWidth, endWidth, path.m_uv0Multiplier, path.m_uv1Multiplier);
						
						GameObject newCollider = new GameObject("Collider (" + bezier.m_v0.x + ", " + bezier.m_v0.y + ")");
						newCollider.tag = "EditorOnly";
						newCollider.transform.parent = collidersObject.transform;
							
						MeshCollider meshCollider = newCollider.AddComponent<MeshCollider>();
						meshCollider.sharedMesh = colliderMesh;
							
					}
					
					count++;
				}
			}
		}
		
		GUILayout.EndVertical();
	}
	
	public void OnSceneGUI()
	{
		Path path = (Path)target;
		
		SplineEditor.DrawEditor(path.m_spline, path.m_drawPathOnly);	
		
		float length = path.m_spline.GetLength();
		
		Handles.color = Color.red;
		Handles.DrawLine(path.m_spline.m_beziers[0].m_v0, path.m_spline.m_beziers[0].m_v0 + new Vector2(length, 0.0f));
	}
}
