using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TreeRender : MonoBehaviour 
{
	public float animDuration = 1.0f;
	public Color baseColour  = new Color(0.6f, 0.6f, 0.15f, 1.0f);
	
	MeshRenderer m_renderer;
	
	// Use this for initialization
	void Start () 
	{
		m_renderer = GetComponent<MeshRenderer>();
		m_renderer.sharedMaterial.SetVector("_BaseColour", baseColour);
	}
	
	private Vector4 m_progress = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
	private float progress = 0.0f;
	
	private bool m_increasing = true;
	
	// Update is called once per frame
	void Update () 
	{
		if(m_increasing) 
		{
			progress += Time.deltaTime / animDuration;
			if(progress >= 1.0f)
			{
				m_increasing = false;
				progress = 1.0f;
			}
		}
		else
		{
			progress -= Time.deltaTime / animDuration;
			if(progress <= 0.0f)
			{
				m_increasing = true;
				progress = 0.0f;
			}
		}
		
		float change = Mathf.Sin(progress);
		
		
		m_progress = new Vector4(change, 1.0f - change,1.0f, 0.0f);
		
		m_renderer.material.SetVector("_ShadeProgress", m_progress);
	}
	
#if UNITY_EDITOR 
	public void UpdateBaseMaterial()
	{
		Material newMaterial = m_renderer.material;
		newMaterial.SetVector("_BaseColour", baseColour);
		m_renderer.material = newMaterial;
	}
#endif
	
}
