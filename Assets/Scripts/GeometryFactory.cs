using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof (MeshFilter))]
public class GeometryFactory : MonoBehaviour 
{
	public enum GeometryType
	{
		Plane,	
		Triangle
	}
	
	public GeometryType geometryType = GeometryType.Plane;
	public bool ScaleUVs = false;
	public float UVScale = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
		MeshFilter meshFilterComponent = GetComponent<MeshFilter>();
		
		Mesh newMesh = null;
		/*
		switch(geometry_type)
		{
			case GeometryType.Plane: newMesh = CreatePlane(ScaleUVsToDimensions ? transform.localScale.x : 1.0f); break;
		}
		
		meshFilterComponent.mesh = newMesh;
		*/
	}
	
	
}
