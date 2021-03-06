/// <summary>
/// Sprite animation set.
/// 
/// Contains a collection of SpriteAnimation instances and is responsible for tracking
///  which animation is active at a given time.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SpriteAnimationSet
{
	public bool Load(string dataPath)
	{
		m_animationDictionary.Clear();
		
		XmlDocument spriteDoc = new XmlDocument();
		
		TextAsset spriteAsset = AssetHelper.Instance.GetAsset<TextAsset>(dataPath) as TextAsset;
		if(spriteAsset != null)
		{
			spriteDoc.LoadXml(spriteAsset.text);
		}
		else
		{
			Debug.LogWarning("Failed to load sprite data: " + dataPath);	
			return false;	
		}
		
		XmlNodeList animationNodes = spriteDoc.GetElementsByTagName("animation");
		m_animations = new SpriteAnimation[animationNodes.Count];
		
		int currentNode = 0;
		foreach(XmlNode node in animationNodes)
		{
			m_animations[currentNode] = new SpriteAnimation();
			m_animations[currentNode].Load(node);
			
			m_animationDictionary.Add(m_animations[currentNode].Name, currentNode);
			
			currentNode++;
		}
		
		if(m_animations.Length > 0)
		{
			CurrentAnimation = m_animations[0];
		}
		
		return true;
	}
	
	public void PlayAnimation(string animName)
	{
		if(animName == CurrentAnimation.Name)
		{
			return;	
		}
		
		int index = -1;
		m_animationDictionary.TryGetValue(animName, out index);
		
		if(index != -1)
		{
			CurrentAnimation = m_animations[index];
			Advance();
		}
		else
		{
			Debug.LogWarning("Sprite Animation \"" + animName + "\" not found");	
		}
	}
	
	public Texture2D SpriteSetTexture {	get; set; }
	
	public float GetAnimationSpeed()
	{
		return speed;
	}
	
	public Vector4 Advance()
	{
		if(CurrentAnimation != null)
		{
			return CurrentAnimation.Advance();
		}
		
		return Vector4.one;
	}
	
	public SpriteAnimation CurrentAnimation { get; set; }
	
	private Texture2D m_texture;
	private float speed = 0.38f; 
	private SpriteAnimation[] m_animations;
	private Dictionary<string, int> m_animationDictionary = new Dictionary<string, int>();
}