///////////////////////////////////////////////////////////
// 
// MathsHelper.cs
//
// What it does: Some cruddy 2D maths bits
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class MathsHelper
{
	/// <summary>
	/// Returns whether a line-segment intersects a triangle in 2D.
	/// First two parameters are the line start and end. Other three are triangle verts
	/// </summary>
	public static bool LineTriIntersect(Vector2 l0, Vector2 l1, Vector2 t0, Vector2 t1, Vector2 t2)
	{
		bool intersect = false;
		
		Vector2 intersectionPoint;
		
		// TODO: Can early out here
		intersect |= LineIntersectionPoint(l0, l1, t0, t1, out intersectionPoint);
		intersect |= LineIntersectionPoint(l0, l1, t2, t1, out intersectionPoint);
		intersect |= LineIntersectionPoint(l0, l1, t0, t2, out intersectionPoint);
		
		return intersect;
	}
	
	/// <summary>
	/// Returns whether a line is contained within a triangle in 2D.
	/// </summary>
	public static bool LineInTri(Vector2 l0, Vector2 l1, Vector2 t0, Vector2 t1, Vector2 t2)
	{
		// Simply check to see if both point are inside the triangle
		
		bool l0InTri0 = sign(l0, t0, t1) <= 0.0f;
		bool l0InTri1 = sign(l0, t1, t2) <= 0.0f;
		bool l0InTri2 = sign(l0, t2, t0) <= 0.0f;
		
		bool l1InTri0 = sign(l1, t0, t1) <= 0.0f;
		bool l1InTri1 = sign(l1, t1, t2) <= 0.0f;
		bool l1InTri2 = sign(l1, t2, t0) <= 0.0f;
		
		bool l0InTri = ((l0InTri0 == l0InTri1) && (l0InTri1 == l0InTri2));
		bool l1InTri = ((l1InTri0 == l1InTri1) && (l1InTri1 == l1InTri2));
		
		return (l0InTri && l1InTri);
	}
	
	// Just a truncated cross product to get the z-val
	public static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
	{
	  return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	} 
		
	public static bool LineIntersectionPoint(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, out Vector2 intersectionPoint)
	{
		intersectionPoint = Vector2.zero;
		
		bool intersection = false;
		//bool coincident = false;
		
		float ua = (v3.x - v2.x) * (v0.y - v2.y) - (v3.y - v2.y) * (v0.x - v2.x);
		float ub = (v1.x - v0.x) * (v0.y - v2.y) - (v1.y - v0.y) * (v0.x - v2.x);
		float denominator = (v3.y - v2.y) * (v1.x - v0.x) - (v3.x - v2.x) * (v1.y - v0.y);
		
		intersection = /* coincident = */ false;
		
		if (Mathf.Abs(denominator) <= 0.00001f)
		{
		    if (Mathf.Abs(ua) <= 0.00001f && Mathf.Abs(ub) <= 0.00001f)
		    {
		        intersection = /* coincident = */ true;
		        intersectionPoint = (v0 + v1) / 2;
		    }
		}
		else
		{
		    ua /= denominator;
		    ub /= denominator;
		
		    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
		    {
		        intersection = true;
		        intersectionPoint.x = v0.x + ua * (v1.x - v0.x);
		        intersectionPoint.y = v0.y + ua * (v1.y - v0.y);
		    }
		}
		return intersection;
	}
}
