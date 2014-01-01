///////////////////////////////////////////////////////////
// 
// NoiseRipple.cs
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

[RequireComponent(typeof(MeshRenderer))]
public class NoiseRipple : PoolableObject 
{
	public float duration = 1.5f;
	public float maxScale = 10.0f;
	
	void Start ()
	{
		m_initialScale = transform.localScale;
	}

	void Update ()
	{
		float delta = ((maxScale - m_initialScale.x) / duration) * Time.deltaTime;
		transform.rotation = Quaternion.identity;
		transform.localScale += new Vector3(delta, delta, delta);
		
		if(transform.localScale.x > maxScale)
		{
			transform.localScale = m_initialScale;
			m_pool.DeactivateObject(this.gameObject);
		}
		renderer.material.SetFloat("_RippleIntensity", 1.0f - (transform.localScale.x / maxScale));
	}
	
	Vector3 m_initialScale;
}
