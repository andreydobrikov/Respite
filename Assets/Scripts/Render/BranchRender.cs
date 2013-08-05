using UnityEngine;
using System.Collections;

public class BranchRender : MonoBehaviour 
{
	public float animDuration = 1.0f;
	
	MeshRenderer m_renderer;
	
	// Use this for initialization
	void Start () 
	{
		m_renderer = GetComponent<MeshRenderer>();
		
		m_progress = Random.value;
	}
	
	private bool m_increasing = true;
	private float m_progress = 0.0f;
	
	// Update is called once per frame
	void Update () 
	{
		if(m_increasing) 
		{
			m_progress += Time.deltaTime / animDuration;
			if(m_progress >= 1.0f)
			{
				m_increasing = false;
				m_progress = 1.0f;
			}
		}
		else
		{
			m_progress -= Time.deltaTime / animDuration;
			if(m_progress <= 0.0f)
			{
				m_increasing = true;
				m_progress = 0.0f;
			}
		}
		
		float change = Mathf.Sin(m_progress);
		
		
		
		m_renderer.material.SetFloat("_Progress", change);
	}
	
}
