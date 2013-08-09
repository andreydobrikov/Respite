using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour 
{

	public void Activate()
	{
		gameObject.SetActive(true);	
		m_activating = true;
	}
	
	public void Deactivate()
	{
		m_activating = false;
		
	}
	
	void Update()
	{
		if(m_activating)
		{
			m_lerpProgress += Time.deltaTime * fadeMultiplier;
		}
		else
		{
			m_lerpProgress -= Time.deltaTime * fadeMultiplier;
		}
		
		m_lerpProgress = Mathf.Clamp(m_lerpProgress, 0.0f, 1.0f);
		
		Vector4 color = renderer.material.GetColor("_Color");
		color.w = m_lerpProgress;
		
		renderer.material.SetColor("_Color", color);
		
		
		if(m_lerpProgress <= 0.0f && !m_activating)
		{
			m_lerpProgress = 0.0f;
			gameObject.SetActive(false);		
		}
	}
	
	private bool m_activating = false;
	private float m_lerpProgress = 0.0f;
	public static float fadeMultiplier = 2.0f;
}
