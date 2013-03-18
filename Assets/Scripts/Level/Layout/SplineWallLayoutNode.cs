using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SplineWallLayoutNode : LayoutNode 
{
	public SplineWallLayoutNode()
	{
		m_start = new Vector3(m_position.x - 1.0f, m_position.y, 0.0f);
		m_end = new Vector3(m_position.x + 1.0f, m_position.y, 0.0f);
	}
	
	public override List<GameObject> BuildObject()
	{
		List<GameObject> objects = new List<GameObject>();
		
		return objects;
	}
	
	public override bool OnGUIInternal(EditType editType, int id, bool selectedNode)
	{
		switch(editType)
		{
			case EditType.Move: 
			{
				m_startTangent = Handles.Slider2D(m_start + m_startTangent, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - m_start;
				m_endTangent = Handles.Slider2D(m_end + m_endTangent, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - m_end;
			
				Handles.DrawLine(m_start, m_start + m_startTangent);
				Handles.DrawLine(m_end, m_end + m_endTangent);
		
				m_start = Handles.Slider2D(m_start, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CubeCap, new Vector2(0.5f, 0.5f));
				m_end = Handles.Slider2D(m_end, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.2f, Handles.CubeCap, new Vector2(0.5f, 0.5f));
			
				Handles.DrawBezier(m_start, m_end, m_start + m_startTangent, m_end + m_endTangent, Color.red, null, 3.0f);
			
				break;
			}
			case EditType.Link:
			{
				Handles.Button(m_start, Quaternion.identity, 0.2f, 0.2f, Handles.SphereCap);
				Handles.Button(m_end, Quaternion.identity, 0.2f, 0.2f, Handles.SphereCap);
				break;
			}
		}
		return false;
	}
	
	[SerializeField]
	protected Vector3 m_start;
	
	[SerializeField]
	protected Vector3 m_end;
	
	[SerializeField]
	protected Vector3 m_startTangent;
	
	[SerializeField]
	protected Vector3 m_endTangent;
}
