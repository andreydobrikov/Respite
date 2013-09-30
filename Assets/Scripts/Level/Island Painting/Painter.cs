///////////////////////////////////////////////////////////
// 
// IPainter.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Painter : MonoBehaviour, IComparable
{
	public int Index
	{
		get { return m_index; }
		set { m_index = value; }
	}
	
	public bool ShouldPaint
	{
		get { return m_shouldPaint; }
		set { m_shouldPaint = value; }
	}
	
	public abstract void Paint(Island island);
	public abstract string GetName();
	
	public int CompareTo(object that)
	{
		Painter other = (Painter)that;
		
		if(m_index == other.Index)
		{
			return 0;
		}
		
		if(m_index > other.Index)
		{
			return 1;	
		}
		
		return -1;
	}
	
	[SerializeField]
	private int m_index = 0;
	
	[SerializeField]
	private bool m_shouldPaint = true;
}
