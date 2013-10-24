using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class SpriteData
{
	public bool Load(string dataPath)
	{
		XmlDocument spriteDoc = new XmlDocument();
		FileInfo info = new FileInfo(dataPath);
		if(info.Exists)
		{
			spriteDoc.Load(dataPath);
			
			XmlNodeList animationNodes = spriteDoc.GetElementsByTagName("animation");
			m_animations = new SpriteAnimation[animationNodes.Count];
			
			int currentNode = 0;
			foreach(XmlNode node in animationNodes)
			{
				m_animations[currentNode] = new SpriteAnimation();
				m_animations[currentNode].Load(node);
				
				currentNode++;
			}
			
			if(m_animations.Length > 0)
			{
				CurrentAnimation = m_animations[0];
			}
			
			return true;
		}
		
		return false;
	}
	
	public void SetTexture(Texture2D texture)
	{
		m_texture = texture;	
	}
	
	public float GetAnimationSpeed()
	{
		return speed;
	}
	
	public SpriteAnimation CurrentAnimation
	{
		get { return m_currentAnimation; }
		set { m_currentAnimation = value; }
	}
	
	Texture2D m_texture; 
	
	float speed = 0.38f; 
	
	SpriteAnimation m_currentAnimation = null;
	SpriteAnimation[] m_animations;
}