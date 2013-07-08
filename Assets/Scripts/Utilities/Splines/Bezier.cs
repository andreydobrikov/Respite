/// <summary>
/// Utility functions for cubic beziers. 
/// 
/// Not optimised in any form. Bugger off. Git.
/// </summary>

using UnityEngine;

public class Bezier
{
	/// <summary>
	/// Gets a point on the specified bezier.
	/// </summary>
	public static Vector2 GetBezierPoint(float progress, Vector2 v0, Vector2 t0, Vector2 v1, Vector2 t1)
	{
		// I'm 99% certain that this can be simplified down.
		// I'm certain of this because I did it to get the first derivative used below for the tangent.
		// Unfortunately, I can't find the paper upon which I wrote it. So, fuck it.
		Vector2 p0 = ((1.0f - progress) * v0) + (progress * t0);
		Vector2 p1 = ((1.0f - progress) * t0) + (progress * t1);
		Vector2 p2 = ((1.0f - progress) * t1) + (progress * v1);
		
		Vector2 s0 = ((1.0f - progress) * p0) + (progress * p1);
		Vector2 s1 = ((1.0f - progress) * p1) + (progress * p2);
		
		Vector2 result = ((1.0f - progress) * s0) + (progress * s1);
		
		return result;
	}
	
	/// <summary>
	/// Gets the normal to the specified bezier at a particular point.
	/// </summary>
	public static Vector2 GetBezierNormal(float progress, Vector2 v0, Vector2 t0, Vector2 v1, Vector2 t1)
	{
		Vector2 tangent = 	((-3.0f * ((1.0f - progress) * (1.0f - progress))) * v0) + 
							((3.0f * ((1.0f - progress) * (1.0f - progress))) * t0) - 
							(((6.0f * progress) * (1.0f - progress)) * t0) - 
							((3.0f * (progress * progress)) * t1) + 
							(((6.0f * progress) * (1.0f - progress)) * t1) + 
							((3.0f * (progress * progress)) * v1);

		tangent.Normalize();
		
		Vector3 normal3D = Vector3.Cross(new Vector3(0.0f, 0.0f, -1.0f), tangent); 
		Vector2 normal = new Vector2(normal3D.x, normal3D.y);
		
		return normal;
	}
	
	/// <summary>
	/// Apparently this can be approximated just as effectively with a single extrapolation of one 
	/// segment. This sits badly with my stupid intuition, so I do a low-resolution pass over the whole bezier.
	/// </summary>
	public static float GetBezierLength(Vector2 v0, Vector2 t0, Vector2 v1, Vector2 t1)
	{
		const int approximationDetail = 10;
		float incrementSize = 1.0f / ((float)(approximationDetail - 1));
		
		float progress = 0.0f;
		
		for(int segment = 0; segment < (approximationDetail - 1); segment++)
		{
			float s0 = (float)segment * incrementSize;
			float s1 = (float)(segment + 1) * incrementSize;
			
			Vector2 p0 = GetBezierPoint(s0, v0, t0, v1, t1);	
			Vector2 p1 = GetBezierPoint(s1, v0, t0, v1, t1);	
			
			Vector2 diff = p1 - p0;
			progress += Mathf.Abs(diff.magnitude); 
		}
		
		return (progress / (float)(approximationDetail-1)) ;
	}
	
	public static Mesh GetBezierMesh(Vector2 v0, Vector2 t0, Vector2 v1, Vector2 t1, int iterations, bool hasDepth)
	{
		Mesh nodeMesh = new Mesh();
		Vector3[] 	vertices 	= new Vector3[(iterations * 2) * 2]; // +12 for the end caps
		Vector2[] 	uvs 		= new Vector2[(iterations * 2) * 2];
		int[] 		triangles 	= new int[(iterations - 1) * 24 + 24];
				
				float wallSize = 0.2f;
				float stepSize = 1.0f / (float)(iterations - 1);
				Vector2 lastPoint = Bezier.GetBezierPoint(0.0f, v0, t0, v1, t1);
				float accumulatedDistance = 0.0f;
				for(int i = 0; i < iterations; i++)
				{
					Vector2 bezierPoint = Bezier.GetBezierPoint(stepSize * (float)i, v0, t0, v1, t1);
					Vector2 tangent = Bezier.GetBezierNormal(stepSize * (float)i, v0, t0, v1, t1);
					
					vertices[i * 4] = new Vector3(bezierPoint.x, bezierPoint.y, 0.0f);
					vertices[i * 4] += new Vector3(tangent.x, tangent.y) * wallSize;
					
					vertices[i * 4 + 1] = new Vector3(bezierPoint.x, bezierPoint.y, 0.0f);
					vertices[i * 4 + 1] += new Vector3(-tangent.x, -tangent.y) * wallSize;
					
					vertices[i * 4 + 2] = new Vector3(bezierPoint.x, bezierPoint.y, 1.0f);
					vertices[i * 4 + 2] += new Vector3(tangent.x, tangent.y) * wallSize;
					
					vertices[i * 4 + 3] = new Vector3(bezierPoint.x, bezierPoint.y, 1.0f);
					vertices[i * 4 + 3] += new Vector3(-tangent.x, -tangent.y) * wallSize;
					
			
					/* Removing this to hush warnings. It necessarily can't have been doing anything anyway.
					if(lastPoint == null)
					{
						uvs[i * 4] = new Vector2( 0.0f, 0.0f);
						uvs[i * 4 + 1] = new Vector2( 0.0f, 1.0f);	
						uvs[i * 4 + 2] = new Vector2( 0.0f, 0.0f);
						uvs[i * 4 + 3] = new Vector2( 0.0f, 1.0f);	
					}
					else*/
					{
						Vector2 toLastPoint = bezierPoint - lastPoint;
						float distanceToLastPoint = toLastPoint.magnitude;
						
						accumulatedDistance += distanceToLastPoint;
						
						uvs[i * 4] = new Vector2( accumulatedDistance, 0.0f) ;
						uvs[i * 4 + 1] = new Vector2( accumulatedDistance, 1.0f);	
						uvs[i * 4 + 2] = new Vector2( accumulatedDistance, 0.0f) ;
						uvs[i * 4 + 3] = new Vector2( accumulatedDistance, 1.0f);	
					}
					lastPoint = bezierPoint;
					
				}

				for(int i = 0; i < iterations - 1; i++)
				{
					//Top
					triangles[i * 24] = i * 4;
					triangles[i * 24 + 1] = i * 4 + 1;
					triangles[i * 24 + 2] =  i * 4 + 4;
					
					triangles[i * 24 + 3] = i * 4 + 4;
					triangles[i * 24 + 4] = i * 4 + 1;
					triangles[i * 24 + 5] = i * 4 + 5;
					
					// Bottom
					triangles[i * 24 + 6] = i * 4 + 2;
					triangles[i * 24 + 7] = i * 4 + 6;
					triangles[i * 24 + 8] =  i * 4+ 3;
					
					triangles[i * 24 + 9] = i * 4+ 6;
					triangles[i * 24 + 10] = i * 4+ 7;
					triangles[i * 24 + 11] = i * 4+ 3;
					
					// Left
					triangles[i * 24 + 12] = i * 4;
					triangles[i * 24 + 13] = i * 4+ 4;
					triangles[i * 24 + 14] =  i * 4+ 2;
					
					triangles[i * 24 + 15] = i * 4+ 2;
					triangles[i * 24 + 16] = i * 4+ 4;
					triangles[i * 24 + 17] = i * 4+ 6;
					
					// Right
					triangles[i * 24 + 18] = i * 4+ 1;
					triangles[i * 24 + 19] = i * 4+ 3;
					triangles[i * 24 + 20] =  i * 4+ 5;
					
					triangles[i * 24 + 21] = i * 4+ 5;
					triangles[i * 24 + 22] = i * 4+ 3;
					triangles[i * 24 + 23] = i * 4+ 7;
				}
				
				// End caps
				int capStart = (iterations - 1) * 24;
				triangles[capStart] = 0;
				triangles[capStart + 1] = 2;
				triangles[capStart + 2] = 1;
				triangles[capStart + 3] = 2;
				triangles[capStart + 4] = 3;
				triangles[capStart + 5] = 1;
				
				int offset = vertices.Length - 4;
				triangles[capStart + 6] = offset + 0;
				triangles[capStart + 7] = offset + 1;
				triangles[capStart + 8] = offset + 2;
				triangles[capStart + 9] = offset + 2;
				triangles[capStart + 10] = offset + 1;
				triangles[capStart + 11] = offset + 3;
				
				nodeMesh.vertices 	= vertices;
				nodeMesh.uv 			= uvs;
				nodeMesh.triangles 	= triangles;	
		
		return nodeMesh;
	}
}
