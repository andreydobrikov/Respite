///////////////////////////////////////////////////////////
// 
// Island.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Island : MonoBehaviour 
{
	public int SectionsX = 10;
	public int SectionsY = 10;
	
	
	public Mesh IslandSourceMesh = null;
	
	public List<MeshSlice.Triangle> m_triangles = new List<MeshSlice.Triangle>();
	
	public Vector2 sliceStart = Vector2.zero;
	public Vector2 sliceEnd = Vector2.one;
}
