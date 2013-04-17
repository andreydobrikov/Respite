using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class TerrainGlow : MonoBehaviour 
{
	public float MinBlend = 0.8f;
	public float MaxBlend = 1.0f;
	public float CycleTime = 1.0f;
	
	public float AnimX = 0.2f;
	public float AnimY = 0.2f;
	
	private float m_currentProgress = 0.0f;
	private bool m_increasing = true;
	private float m_animX = 0.0f;
	private float m_animY = 0.0f;
	
	MeshRenderer meshRenderer = null;
	// Use this for initialization
	void Start () 
	{
		meshRenderer = GetComponent<MeshRenderer>();
		m_currentProgress = MinBlend;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float deltaRate = CycleTime / (MaxBlend - MinBlend);
		deltaRate = Time.deltaTime / deltaRate;
		
		if(m_increasing)
		{
			m_currentProgress += deltaRate;
			if(m_currentProgress >= MaxBlend)
			{
				m_increasing = false;	
			}
		}
		else
		{
			m_currentProgress -= deltaRate;
			if(m_currentProgress <= MinBlend)
			{
				m_increasing = true;	
			}
		}
		
		m_animX += AnimX;
		m_animY += AnimY;
		
		meshRenderer.material.SetFloat("_ShadowFactor", m_currentProgress);
		meshRenderer.material.SetTextureOffset("_ShadowTex", new Vector2(m_animX, m_animY));
	}
}
