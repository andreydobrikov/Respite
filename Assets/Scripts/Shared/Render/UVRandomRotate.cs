///////////////////////////////////////////////////////////
// 
// UVRandomRotate.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class UVRandomRotate : MonoBehaviour 
{
	public float UpdateRate = 0.1f;
	
	[ShaderPropertyNameAttribute(ShaderPropertyNameAttribute.PropertyType.TexEnv)]
	public string TargetTexture = null;
	
	private MeshRenderer m_renderer = null;

	void Start ()
	{
		m_renderer = GetComponent<MeshRenderer>();
	}

	void Update ()
	{
		m_updateProgress += GameTime.DeltaTime;
		
		if(m_updateProgress > UpdateRate)
		{
			m_updateProgress = 0.0f;
		
			m_uvScroll.x = Random.value;
			m_uvScroll.y = Random.value;
			
			m_renderer.sharedMaterial.SetTextureOffset(string.IsNullOrEmpty(TargetTexture) ? "_MainTex" : TargetTexture, m_uvScroll);
		}
	}
	
	private Vector2 m_uvScroll = Vector2.one;
	private float m_updateProgress = 0.0f;
}
