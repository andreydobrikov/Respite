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
		
		if(newType == GeometryFactory.GeometryType.ScreenPlane)
		{
			myTarget.ScreenPlaneTargetCamera = EditorGUILayout.ObjectField(myTarget.ScreenPlaneTargetCamera, typeof(Camera), true) as Camera;	
		}
		
		bool newScaleValue = EditorGUILayout.Toggle("Scaled World-Space UVs 0", myTarget.ScaleUVs);
		
		float NewUVScale0 = myTarget.UVScale0;
		float NewUVScale1 = myTarget.UVScale1;
		
		if(newScaleValue)
		{
			NewUVScale0 = EditorGUILayout.FloatField("UV 0", myTarget.UVScale0);	
			NewUVScale1 = EditorGUILayout.FloatField("UV 1", myTarget.UVScale1);
		}
		
		if(newType != myTarget.geometryType || newScaleValue != myTarget.ScaleUVs || NewUVScale0 != myTarget.UVScale0 || NewUVScale1 != myTarget.UVScale1)
		{
			myTarget.geometryType = newType;	
			myTarget.ScaleUVs = newScaleValue;
			myTarget.UVScale0 = NewUVScale0;
			myTarget.UVScale1 = NewUVScale1;
			
			MeshFilter mesh = myTarget.GetComponent<MeshFilter>();
			if(mesh != null)
			{
				if(myTarget.ScaleUVs)
				{
					mesh.sharedMesh = CreatePlane(	myTarget.transform.lossyScale.x * myTarget.UVScale0, 
													myTarget.transform.lossyScale.z * myTarget.UVScale0,
													myTarget.transform.lossyScale.x * myTarget.UVScale1,
													myTarget.transform.lossyScale.z * myTarget.UVScale1);
				}
				else
				{
					mesh.sharedMesh = CreatePlane(1.0f, 1.0f, 1.0f, 1.0f);		
				}
			}
		}
		
		
		EditorUtility.SetDirty (myTarget);
	}
	
	public static Mesh CreatePlane(float UV0XScale, float UV0YScale, float UV1XScale, float UV1YScale)
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector3[] 	normals 	= new Vector3[4];
		Vector2[] 	uvs0 		= new Vector2[4];
		Vector2[] 	uvs1 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, 0.0f, -0.5f);
		vertices[1] = new Vector3(0.5f, 0.0f, -0.5f);
		vertices[2] = new Vector3(-0.5f, 0.0f, 0.5f);
		vertices[3] = new Vector3(0.5f, 0.0f, 0.5f);
		
		normals[0] = new Vector3(0.0f, 1.0f, 0.0f);
		normals[1] = new Vector3(0.0f, 1.0f, 0.0f);
		normals[2] = new Vector3(0.0f, 1.0f, 0.0f);
		normals[3] = new Vector3(0.0f, 1.0f, 0.0f);
		
		uvs0[0] = new Vector2(0.0f, 0.0f);
		uvs0[1] = new Vector2(UV0XScale, 0.0f);
		uvs0[2] = new Vector2(0.0f, UV0YScale);
		uvs0[3] = new Vector2(UV0XScale, UV0YScale);
		
		uvs1[0] = new Vector2(0.0f, 0.0f);
		uvs1[1] = new Vector2(UV1XScale, 0.0f);
		uvs1[2] = new Vector2(0.0f, UV1YScale);
		uvs1[3] = new Vector2(UV1XScale, UV1YScale);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices = vertices;
		newMesh.normals = normals;
		newMesh.uv = uvs0;
		newMesh.uv1 = uvs1;
		newMesh.triangles = triangles;
		
		return newMesh;
	}
}
