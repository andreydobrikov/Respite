///////////////////////////////////////////////////////////
// 
// Footprint.cs
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

public class Footprint : PoolableObject 
{
	public float DecayTime = 5.0f;
	public float MaxAlpha = 0.4f;
	
	void Start()
	{
		m_initialTint = Vector4.one;
		m_initialTint.a = MaxAlpha;
	}
	
	void OnEnable()
	{
		m_decay = DecayTime;
		renderer.material.color = m_initialTint;
	}
	
	void Update ()
	{
		m_decay -= Time.deltaTime;
	
		float alpha = Mathf.Lerp(0.0f, m_initialTint.a, m_decay / DecayTime); 
		Vector4 tint = m_initialTint;
		tint.w = alpha;
		
		renderer.material.color = tint;
		
		if(m_decay <= 0.0f)
		{
			m_pool.DeactivateObject(this.gameObject);	
		}
	}
	
	private float m_decay = 5.0f;
	private Color m_initialTint;
}
