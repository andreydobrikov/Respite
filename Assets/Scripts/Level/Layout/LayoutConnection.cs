using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class LayoutConnection : ScriptableObject, IComparable<LayoutConnection>
{
	public enum ConnectionTypes
	{
		Line,
		Bezier
	}
	
	public LayoutConnection()
	{
		m_sourceTangent = new Vector2( 1.0f, 1.0f);
		m_targetTangent = new Vector2( 1.0f, 1.0f);
		
		m_connectionType = ConnectionTypes.Line;
		Built = false;
	}
	
	public LayoutNode Source
	{
		get { return m_sourceNode; }
		set { m_sourceNode = value; }
	}
	
	public LayoutNode Target
	{
		get { return m_targetNode; }	
		set { m_targetNode = value; }
	}
	
	public bool Built
	{
		get; set;	
	}
	
	public ConnectionTypes ConnectionType
	{
		get
		{
			return m_connectionType;
		}
		
		set
		{
			m_connectionType = value;	
			
		}
	}
	
	public Vector2 SourceTangent
	{
		get { return m_sourceTangent; }
		set { m_sourceTangent = value; }
	}
	
	public Vector2 TargetTangent
	{
		get { return m_targetTangent; }
		set { m_targetTangent = value; }
	}
	
	public int BezierSections
	{
		get { return m_bezierSections; }
		set 
		{ 
			m_bezierSections = value; 
			if(m_bezierSections < 3)
			{
				m_bezierSections = 3;	
			}
		}
	}
	
	public int CompareTo(LayoutConnection other)
	{
		if(other == this)
		{
			return 0;	
		}
		
		LayoutNode commonNode = null;
		
		if(other.m_sourceNode == m_sourceNode || other.m_targetNode == m_sourceNode) commonNode = m_sourceNode;
		if(other.m_sourceNode == m_targetNode || other.m_targetNode == m_targetNode) commonNode = m_targetNode;
		
		// No common layout node, so bail.
		if(commonNode == null)
		{
			return 0;	
		}
		
		Vector3 otherDirection = other.m_targetNode == commonNode 
								? (other.m_sourceNode.LocalPosition - other.m_targetNode.LocalPosition) 
								: (other.m_targetNode.LocalPosition - other.m_sourceNode.LocalPosition);
		
		Vector3 thisDirection =  m_targetNode == commonNode 
								? (m_sourceNode.LocalPosition - m_targetNode.LocalPosition) 
								: (m_targetNode.LocalPosition - m_sourceNode.LocalPosition);
		
		otherDirection.Normalize();
		thisDirection.Normalize();
		
		float otherAngle = Mathf.Atan2(otherDirection.x, otherDirection.y);
		float thisAngle = Mathf.Atan2(thisDirection.x, thisDirection.y);
		
		return otherAngle < thisAngle ? 1 : -1;
		
	}
	
	[SerializeField]
	private LayoutNode m_sourceNode;
	
	[SerializeField]
	private LayoutNode m_targetNode;
	
	[SerializeField]
	private Vector2 m_sourceTangent;
	
	[SerializeField]
	private Vector2 m_targetTangent;
	
	[SerializeField]
	private ConnectionTypes m_connectionType;
	
	[SerializeField]
	private int m_bezierSections = 10;
}
