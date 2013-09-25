using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Spline
{
	public Spline()
	{
		m_beziers 			= new Bezier[m_controlPoints];
		m_widthModifiers 	= new float[m_controlPoints + 1];
		
		for(int i = 0; i < m_controlPoints; ++i)
		{
			m_widthModifiers[i] = 1.0f;
		}
	}
	
	public void Start()
	{
		float x = 0.0f;
		float y = 0.0f;
		
		
		for(int i = 0; i < m_controlPoints; ++i)
		{
			m_widthModifiers[i] = 1.0f;
			m_beziers[i] 		= new Bezier();
			
			m_beziers[i].m_v0 = new Vector2(x, y);
			m_beziers[i].m_t0 = new Vector2(x, y);
			
			x += UnityEngine.Random.Range(1.0f, 3.0f);
			y += UnityEngine.Random.Range(1.0f, 3.0f);
			
			m_beziers[i].m_v1 = new Vector2(x, y);
			m_beziers[i].m_t1 = new Vector2(x, y);
		}
		
		m_widthModifiers[m_controlPoints] = 1.0f;
	}
	
	public void ResizeSpline(int controlPointCount)
	{
		int end = Math.Min(controlPointCount, m_controlPoints);
		
		Bezier[] newBeziers = new Bezier[controlPointCount];
		float[] newModifiers = new float[controlPointCount + 1];
		
		Array.Copy(m_beziers, newBeziers, end);
		
		for(int i = 0; i < end; i++)
		{
			newModifiers[i] = 1.0f;
		}
		
		for(int i = end; i < newBeziers.Length; ++i)
		{
			newModifiers[i] = 1.0f;
			
			Vector2 t0Diff = newBeziers[i - 1].m_t1 - newBeziers[i - 1].m_v1;
			
			newBeziers[i] = new Bezier();
			newBeziers[i].m_v0 = newBeziers[i - 1].m_v1;
			newBeziers[i].m_t0 = newBeziers[i].m_v0 - t0Diff;
			
			newBeziers[i].m_v1 = newBeziers[i].m_v0 + ( (newBeziers[i - 1].m_v1 - newBeziers[i - 1].m_v0).normalized * 2.0f );
			newBeziers[i].m_t1 = newBeziers[i].m_v1;
		}
		
		newModifiers[newBeziers.Length] = 1.0f;
		
		m_beziers = newBeziers;
		m_widthModifiers = newModifiers;
		m_controlPoints = controlPointCount;
	}
	
	public void Offset(Vector2 offset)
	{
		foreach(var bezier in m_beziers)
		{
			bezier.m_v0 += offset;
			bezier.m_v1 += offset;
			bezier.m_t0 += offset;
			bezier.m_t1 += offset; 
		}
	}
	
	public float GetLength()
	{
		float length = 0;
		
		foreach(var bezier in m_beziers)
		{
			length += bezier.GetLength();
		}
		
		return length;
	}
	
	public Vector2 GetPosition(float progress)
	{
		float splineLength = GetLength ();
		float positionLength = progress * splineLength;
		
		float accumulatedLength = 0.0f;
		for(int i = 0; i < m_beziers.Length; i++)
		{
			float bezierLength = m_beziers[i].GetLength();
			
			if(accumulatedLength + bezierLength < positionLength)
			{
				accumulatedLength += bezierLength;	
			}
			else
			{
				// This is distance along the current bezier that the progress will be.
				float localProgress = positionLength - accumulatedLength;
				
				// This is the normalized distance along the current bezier
				float normalizedDistance = 	localProgress / bezierLength;
				//return Vector2.one;
				return m_beziers[i].GetBezierPoint(normalizedDistance);
			}
		}
		
	//	Debug.LogWarning("Requested spline progress is out of bounds! (" + progress + ")");
		return Vector2.zero;	
	}
	
	public float GetWidth(float progress)
	{
		int modifierIndex = 0;
		float splineLength = GetLength ();
		float positionLength = progress * splineLength;
		
		float accumulatedLength = 0.0f;
		for(int i = 0; i < m_beziers.Length; i++)
		{
			float bezierLength = m_beziers[i].GetLength();
			
			if(accumulatedLength + bezierLength < positionLength)
			{
				accumulatedLength += bezierLength;	
				modifierIndex++;
			}
			else
			{
				// This is distance along the current bezier that the progress will be.
				float localProgress = positionLength - accumulatedLength;
				
				// This is the normalized distance along the current bezier
				float normalizedDistance = 	localProgress / bezierLength;
				
				float width = Mathf.Lerp(m_widthModifiers[modifierIndex], m_widthModifiers[modifierIndex + 1], localProgress);
				return width;
			}
		}
		return 1.0f;
	}	
	
	[SerializeField]
	private int m_controlPoints = 10;
	
	[SerializeField]
	public Bezier[] m_beziers;
	
	[SerializeField]
	public float[] m_widthModifiers;
	
	[SerializeField]
	public bool showModifiers = false;
	
}
