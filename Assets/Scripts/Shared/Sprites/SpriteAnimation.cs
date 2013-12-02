/// <summary>
/// SpriteAnimation
/// 
/// Contains all the required data for a specific sprite animation. 
/// The class is responsible for tracking the progress of an animation as well as its frame-rate.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class SpriteAnimation
{
	private class FrameData
	{
		public int frameID = 0;
		public Vector4 m_uvOffset = new Vector4();
	}
	
	public void Reset()
	{
		m_currentFrame = 0;
	}
	
	public bool Load(XmlNode node)
	{
		m_name = node.Attributes.GetNamedItem("name").Value;
		
		XmlNode debugColourNode = node.Attributes.GetNamedItem("debug_colour");
		
		if(debugColourNode != null)
		{
			string colour = debugColourNode.Value;
			string[] elements = colour.Split(',');
			
			float r,g,b;
			
			if(elements.Length == 3)
			{
				bool valid = true;
				valid &= float.TryParse(elements[0], out r);
				valid &= float.TryParse(elements[1], out g);
				valid &= float.TryParse(elements[2], out b);
				
				if(valid)
				{
					m_debugColor.x = r;
					m_debugColor.y = g;
					m_debugColor.z = b;
				}
			}
			
		}
	
		m_frameCount = node.ChildNodes.Count;
		
		m_frames = new FrameData[m_frameCount];
		
		int currentFrame = 0;

		// Parse per-frame data
		foreach(XmlNode frameNode in node.ChildNodes)
		{
			FrameData data = new FrameData();
			
			XmlNode xNode = frameNode.Attributes.GetNamedItem("x");
			XmlNode yNode = frameNode.Attributes.GetNamedItem("y");
			XmlNode widthNode = frameNode.Attributes.GetNamedItem("width");
			XmlNode heightNode = frameNode.Attributes.GetNamedItem("height");
			
			if(xNode == null || yNode == null || widthNode == null || heightNode == null)
			{
				Debug.Log("Frame " + currentFrame + " invalid");
				continue;
			}
			
			float.TryParse(xNode.Value, out data.m_uvOffset.x);
			float.TryParse(yNode.Value, out data.m_uvOffset.y);
			float.TryParse(widthNode.Value, out data.m_uvOffset.z);
			float.TryParse(heightNode.Value, out data.m_uvOffset.w);
			
			m_frames[currentFrame] = data;
			currentFrame++;
		}
		
		return true;
	}
	
	public Vector4 CurrentOffset()
	{
		return m_frames[m_currentFrame].m_uvOffset;	
	}
	
	public Vector4 Advance()
	{
		m_currentFrame++;
		m_currentFrame = m_currentFrame % m_frameCount;
		return m_frames[m_currentFrame].m_uvOffset;
	}
	
	public string Name
	{
		get { return m_name; }	
	}
	
	public Vector3 DebugColour
	{
		get { return m_debugColor; }	
	}
	
	// Current animation state
	private int m_currentFrame = 0;
	
	// Static animation values
	private FrameData[] m_frames;
	private int m_frameCount = 0;
	private string m_name;
	private Vector3 m_debugColor = Vector3.one;
}
