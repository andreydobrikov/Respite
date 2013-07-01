using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Rendering/AmbientLightZone")]
public class AmbientLightZone : MonoBehaviour 
{
	public Color TODMinColor = Color.white;
	public Color TODMaxColor = Color.white;
		
	// Use this for initialization
	void Start () 
	{
		m_timeOfDay = FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector4 ambient = m_timeOfDay.TODColor;
		
		ambient = Vector4.Max(ambient, TODMinColor);
		ambient = Vector4.Min(ambient, TODMaxColor);
		
		
		//ambient.Normalize();
		ambient.w = 1.0f;
		
		renderer.material.SetColor("_Color", ambient);
	}
	private TimeOfDay m_timeOfDay = null;
}
