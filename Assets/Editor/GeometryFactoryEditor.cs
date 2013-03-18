using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GeometryFactory))]
public class GeometryFactoryEditor : Editor 
{
	
	
	public override void OnInspectorGUI() 
	{
		GeometryFactory myTarget = (GeometryFactory)target;
		
		GeometryFactory.GeometryType newType  = (GeometryFactory.GeometryType)EditorGUILayout.EnumPopup("Geometry type", myTarget.geometryType);
		bool newScaleValue = EditorGUILayout.Toggle("Scaled World-Space UVs", myTarget.ScaleUVs);
		
		float NewUVScale = myTarget.UVScale;
		if(newScaleValue)
		{
			NewUVScale = EditorGUILayout.FloatField(myTarget.UVScale);	
		}
		
		if(newType != myTarget.geometryType || newScaleValue != myTarget.ScaleUVs || NewUVScale != myTarget.UVScale)
		{
			myTarget.geometryType = newType;	
			myTarget.ScaleUVs = newScaleValue;
			myTarget.UVScale = NewUVScale;
			
			MeshFilter mesh = myTarget.GetComponent<MeshFilter>();
			if(mesh != null)
			{
				if(myTarget.ScaleUVs)
				{
					mesh.sharedMesh = CreatePlane(myTarget.transform.localScale.x * myTarget.UVScale, myTarget.transform.localScale.y * myTarget.UVScale);		
				}
				else
				{
					mesh.sharedMesh = CreatePlane(1.0f, 1.0f);		
				}
			}
		}
		
		
		EditorUtility.SetDirty (myTarget);
	}
	
	public static Mesh CreatePlane(float UVXScale, float UVYScale)
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector2[] 	uvs 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, -0.5f, 0.0f);
		vertices[1] = new Vector3(0.5f, -0.5f, 0.0f);
		vertices[2] = new Vector3(-0.5f, 0.5f, 0.0f);
		vertices[3] = new Vector3(0.5f, 0.5f, 0.0f);
		
		uvs[0] = new Vector2(0.0f, 0.0f);
		uvs[1] = new Vector2(UVXScale, 0.0f);
		uvs[2] = new Vector2(0.0f, UVYScale);
		uvs[3] = new Vector2(UVXScale, UVYScale);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
}
