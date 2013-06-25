using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Rendering/AmbientLightZone")]

public class AmbientLightZone : MonoBehaviour 
{
	public List<AmbientContributor> m_contributors = new List<AmbientContributor>();
		
	// Use this for initialization
	void Start () 
	{
		m_color = renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector4 ambient = Vector4.zero;
		
		foreach(var contributor in m_contributors)
		{
			ambient += contributor.GetAmbientContribution();		
		}
		
		//ambient.Normalize();
		ambient.w = 1.0f;
		
		renderer.material.SetColor("_Color", ambient);
	}
	
	void OnTriggerEnter(Collider other)
	{
	Debug.Log("Trigger");
		AmbientContributor contributor = other.GetComponent<AmbientContributor>();
		if(contributor != null)
		{
			m_contributors.Add(contributor);	
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		AmbientContributor contributor = other.GetComponent<AmbientContributor>();
		if(contributor != null)
		{
			m_contributors.Remove(contributor);	
		}
	}
	
	
	private Vector4 m_color = Vector4.one;
}
