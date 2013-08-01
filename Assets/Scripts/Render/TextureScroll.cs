using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class TextureScroll : MonoBehaviour 
{
	public float UScrollRate = 0.01f;
	public float VScrollRate = 0.01f;
	
	[ShaderPropertyNameAttribute(ShaderPropertyNameAttribute.PropertyType.TexEnv)]
	public string TargetTexture = null;
	
	private MeshRenderer m_renderer = null;
	private Vector2 m_uvScroll = new Vector2();
	
	// Use this for initialization
	void Start () 
	{
		m_renderer = GetComponent<MeshRenderer>();
	}
	
	void FixedUpdate () 
	{
		m_uvScroll += new Vector2(UScrollRate, VScrollRate);
		m_renderer.sharedMaterial.SetTextureOffset(string.IsNullOrEmpty(TargetTexture) ? "_MainTex" : TargetTexture, m_uvScroll);
	}
}
