using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class LightFlicker : MonoBehaviour 
{
	private Vector4 m_initialColour;
	private Vector4 m_flickerColour;
	
	// Use this for initialization
	void Start () 
	{
		
		m_initialColour = renderer.sharedMaterial.GetColor("_Color");
		
		m_flickerColour = m_initialColour * 0.8f;
		m_flickerColour.w = 1.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Random.value > 0.99f)
		{
			m_dimCount = Random.Range(1, 20);	
		}
		
		if(m_dimCount > 0)
		{
			m_dimCount--;
			renderer.material.SetColor("_Color", m_flickerColour);
		}
		else
		{
			renderer.material.SetColor("_Color", m_initialColour);
		}
	}
	
	private int m_dimCount = 0;
}
