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
	
	public Vector4 Advance()
	{
		m_currentFrame++;
		m_currentFrame = m_currentFrame % m_frameCount;
		
		return m_frames[m_currentFrame].m_uvOffset;
	}
	
	// Current animation state
	private int m_currentFrame = 0;
	
	// Static animation values
	private FrameData[] m_frames;
	private int m_frameCount = 0;
	
	string m_name;
}
