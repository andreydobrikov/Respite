///////////////////////////////////////////////////////////
// 
// MeshSlice.cs
//
// What it does: Slices a mesh up into a 2D grid. Really badly.
//
// Notes: Whenever a triangle is sliced using this naive algorithm, you end up with a resultant triangle and a trapezoid.
//		  If you slice from left to right (or right to left), trapezoid density ramps up geometrically.
//		  The best approach would be to write a better algorithm, but an intermediate measure is to do a binary cut of sorts, 
//		  so the density is halved at worst.
// 
// To-do: Sort a better algorithm. This one is shit.
//		  No vertex sharing at the mo.
//		  Vertex attribute lerping. No UVs, normals, etc yet. Pretty trivial.
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MeshSlice 
{
	public static Mesh[,] Slice(Mesh input, int sectionsX, int sectionsY)
	{
		Mesh[,] output = new Mesh[sectionsX, sectionsY];
		
		// Mangle all the Mesh's triangle information into slice-friendly Triangle objects
		List<Triangle> triangles = new List<Triangle>();
		
		for(int i = 0; i < input.triangles.Length / 3; i++)
		{
			Triangle newTri = new Triangle();
			
			newTri.p0 = new Vector2(input.vertices[input.triangles[i * 3]].x, input.vertices[input.triangles[i * 3]].z);
			newTri.p1 = new Vector2(input.vertices[input.triangles[i * 3 + 1]].x, input.vertices[input.triangles[i * 3 + 1]].z);
			newTri.p2 = new Vector2(input.vertices[input.triangles[i * 3 + 2]].x, input.vertices[input.triangles[i * 3 + 2]].z);
			
			newTri.uv0 = input.uv[input.triangles[i * 3]];
			newTri.uv1 = input.uv[input.triangles[i * 3 + 1]];
			newTri.uv2 = input.uv[input.triangles[i * 3 + 2]];
			
			triangles.Add(newTri);
		}
		
		Debug.Log("Located " + triangles.Count + " triangles");
		
		// Ignoring Y for now.
		
		// Unused intersection points from intersection tests. Hear to appease the functions
		Vector2 inter0, inter1;
		
		float xDelta = input.bounds.size.x / (float)sectionsX;
		float yDelta = input.bounds.size.z / (float)sectionsY;
		
		
		for(int x = 1; x < sectionsX; x++)
		{
			List<Triangle> newTris = new List<Triangle>();
			
			float sliceX = input.bounds.min.x + (x * xDelta);
			
			Vector2 sliceStart = new Vector2(sliceX, input.bounds.min.z- 1.0f);
			Vector2 sliceEnd = new Vector2(sliceX, input.bounds.max.z + 1.0f);
			
			foreach(var tri in triangles)
			{
				if(MathsHelper.LineTriIntersect(sliceStart, sliceEnd, tri.p0, tri.p1, tri.p2, out inter0, out inter1))
				{
					//Debug.Log("Slicing... x");
					newTris.AddRange(SliceTri(tri, sliceStart, sliceEnd));
				}
				else
				{
					newTris.Add(tri);	
				}
			}
			triangles = newTris;
		}
		
		for(int y = 1; y < sectionsY; y++)
		{
			List<Triangle> newTris = new List<Triangle>();
			
			float sliceY = input.bounds.min.z + (y * yDelta);
			
			Vector2 sliceStart = new Vector2(input.bounds.min.x- 1.0f, sliceY);
			Vector2 sliceEnd = new Vector2(input.bounds.max.x + 1.0f, sliceY);
			
			foreach(var tri in triangles)
			{
				if(MathsHelper.LineTriIntersect(sliceStart, sliceEnd, tri.p0, tri.p1, tri.p2, out inter0, out inter1))
				{
					//Debug.Log("Slicing... y");
					newTris.AddRange(SliceTri(tri, sliceStart, sliceEnd));
				}
				else
				{
					newTris.Add(tri);	
				}
			}
			triangles = newTris;
		}
		
		Debug.Log("Finished slicing with " + triangles.Count + " tris");
		
		// Okay, so triangles *should* be full of slices triangles
		
		// Blast through each output mesh, gather owned triangles and fling them into the new mesh objects
		for(int i = 0; i < output.GetLength(0); i++)
		{
			for(int j = 0; j < output.GetLength(1); j++)
			{
				List<Triangle> candidateTris = new List<Triangle>();
				
				float xMin = input.bounds.min.x +  i * xDelta;
				float xMax = input.bounds.min.x + (i + 1) * xDelta;
				
				float yMin = input.bounds.min.z + j * yDelta;
				float yMax = input.bounds.min.z + (j + 1) * yDelta;
				
				List<Triangle> remainingTris = new List<Triangle>();
				foreach(var tri in triangles)
				{
					// May have trouble with co-linear degenerates. *shakes fist*
					Vector2 averagePos = (tri.p0 + tri.p1 + tri.p2) / 3.0f;
					//Debug.Log("Average Pos: " + averagePos.x + ", " + averagePos.y);
					
					if(averagePos.x >= xMin && averagePos.x <= xMax && averagePos.y >= yMin && averagePos.y <= yMax)
					{
						candidateTris.Add(tri);
					}
					else
					{
						remainingTris.Add(tri);	
					}
				}
				
				triangles = remainingTris;
					
				Debug.Log("Section " + i + " has " + candidateTris.Count + " tris");
				
				output[i, j] = new Mesh();
				
				Vector3[] verts = new Vector3[candidateTris.Count * 3];
				
				int[] indices = new int[candidateTris.Count * 3];
				Vector2[] uvs = new Vector2[candidateTris.Count * 3];
				
				int count = 0;
				foreach(var tri in candidateTris)
				{
					verts[count] = 	new Vector3(tri.p0.x, 0.0f, tri.p0.y);
					verts[count + 1] = 	new Vector3(tri.p1.x, 0.0f, tri.p1.y);
					verts[count + 2] = 	new Vector3(tri.p2.x, 0.0f, tri.p2.y);
					
					indices[count] = count;
					
					bool thingy = MathsHelper.sign(tri.p0, tri.p1, tri.p2) <= 0.0f;
					
					if(thingy)
					{
						indices[count + 1] = count + 1;
						indices[count + 2] = count + 2;
					}
					else
					{
						indices[count + 1] = count + 2;
						indices[count + 2] = count + 1;
					}
					
					uvs[count] = tri.uv0;
					uvs[count + 1] = tri.uv1;
					uvs[count + 2] = tri.uv2;
					
					count += 3;
				}
				
				output[i, j].vertices = verts;
				output[i, j].triangles = indices;
				output[i, j].uv = uvs;
			}
		}
		
		Debug.LogError("Finished with " + triangles.Count + " triangles unassigned");
		
		return output;
	}
	
	public static Triangle[] SliceTri(Triangle source, Vector2 lineStart, Vector2 lineEnd)
	{
		Triangle[] output = new Triangle[3];
		
		for(int i = 0; i < 3; i++) { output[i] = new Triangle(); }
		
		Vector2 soloPoint;
		Vector2 soloUV;
		
		Vector2 pairPoint0;
		Vector2 pairPoint1;
				Vector2 pairUV0;
		Vector2 pairUV1;
		
		bool sign0 = MathsHelper.sign(source.p0, lineStart, lineEnd) <= 0.0f;
		bool sign1 = MathsHelper.sign(source.p1, lineStart, lineEnd) <= 0.0f;
		bool sign2 = MathsHelper.sign(source.p2, lineStart, lineEnd) <= 0.0f;
		
		if(sign0 == sign1) 		{ soloPoint = source.p2; pairPoint0 = source.p0; pairPoint1 = source.p1; soloUV = source.uv2; pairUV0 = source.uv0; pairUV1 = source.uv1; }
		else if(sign1 == sign2) { soloPoint = source.p0; pairPoint0 = source.p1; pairPoint1 = source.p2; soloUV = source.uv0; pairUV0 = source.uv1; pairUV1 = source.uv2; }
		else 					{ soloPoint = source.p1; pairPoint0 = source.p0; pairPoint1 = source.p2; soloUV = source.uv1; pairUV0 = source.uv0; pairUV1 = source.uv2; }
		
		// Right, so we know which verts are on which side of the slice-line. Time for the intersections.
		
		Vector2 intersection0, intersection1;
		
		
		
		MathsHelper.LineIntersectionPoint(lineStart, lineEnd, pairPoint0, soloPoint, out intersection0);
		MathsHelper.LineIntersectionPoint(lineStart, lineEnd, pairPoint1, soloPoint, out intersection1);
		
		// Lerp dem UVs
		float lerpDistance0 = (intersection0 - pairPoint0).magnitude / (soloPoint - pairPoint0).magnitude;
		float lerpDistance1 = (intersection1 - pairPoint1).magnitude / (soloPoint - pairPoint1).magnitude;
		
		Vector2 intersectionUV0 = pairUV0 + (soloUV - pairUV0) * lerpDistance0;
		Vector2 intersectionUV1 = pairUV1 + (soloUV - pairUV1) * lerpDistance1;
		
		// Three triangles. One for the isolated tri, two for the trapezoid base.
		
		output[0].p0 = soloPoint;
		output[0].p1 = intersection1;
		output[0].p2 = intersection0;
		
		output[0].uv0 = soloUV;
		output[0].uv1 = intersectionUV1;
		output[0].uv2 = intersectionUV0;
		
		Vector2 midLower = pairPoint0 + ((pairPoint1 - pairPoint0) / 2.0f);
		
		output[1].p0 = pairPoint0;
		output[1].p1 = intersection0;
		output[1].p2 = pairPoint1;
		
		output[1].uv0 = pairUV0;
		output[1].uv1 = intersectionUV0;
		output[1].uv2 = pairUV1;
		
		output[2].p0 = pairPoint1;
		output[2].p1 = intersection0;
		output[2].p2 = intersection1;
		
		output[2].uv0 = pairUV1;
		output[2].uv1 = intersectionUV0;
		output[2].uv2 = intersectionUV1;
		
		return output;
	}
			
	
	public class Triangle
	{
		public Vector2 p0, p1, p2;	
		public Vector2 uv0, uv1, uv2;
	}
}
