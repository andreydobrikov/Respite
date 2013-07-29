/// <summary>
/// Lerps a MeshRenderer's material in response to a trigger collision
/// </summary>

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class AlphaTrigger : MonoBehaviour 
{
	[TagAttribute]
	public string TargetTag = string.Empty;
	
	public float CollisionAlpha = 0.2f;
	public float Alpha			= 1.0f;
	public float LerpRate		= 0.01f;
	public float Nudge			= 4.0f;
	
	void Start()
	{
		m_renderer = GetComponent<MeshRenderer>();
		m_colliderRadius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == TargetTag)
		{
			m_playerObject = other.gameObject;
			m_lerpDirection = 1.0f;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag == TargetTag)
		{
			m_playerObject = other.gameObject;
			m_lerpDirection = -1.0f; 
		}
	}
	
	void Update()
	{
		m_lerpProgress += ((LerpRate) * m_lerpDirection);
		
		float max = 1.0f;
		float min = 0.0f;
		
		if(m_playerObject != null)
		{
			float magnitude = (m_playerObject.transform.position - transform.position).magnitude - Nudge;
			max = 1.0f - Mathf.Lerp(0.0f, 1.0f, (magnitude / m_colliderRadius));
		}
		
		m_lerpProgress = Mathf.Clamp(m_lerpProgress, min, max);
		
		float lerpVal = Mathf.Sin(m_lerpProgress * Mathf.PI / 2.0f);
		
		float alpha = Mathf.Lerp(Alpha, CollisionAlpha, lerpVal);
		
		Color current = m_renderer.material.GetColor("_Color");
		current.a = alpha;
			
		//if(m_lerpProgress != 0.0f && m_lerpProgress != 1.0f)
		{
			m_renderer.material.SetColor("_Color", current);
		}
	}
	
	private GameObject m_playerObject = null;
	private float m_colliderRadius = 1.0f;
	private float m_lerpDirection = -1.0f;
	private float m_lerpProgress	= 0.0f;
	private MeshRenderer m_renderer = null;
}
