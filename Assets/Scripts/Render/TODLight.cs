using UnityEngine;
using System.Collections;

public class TODLight : MonoBehaviour 
{
	[ShaderPropertyNameAttribute(ShaderPropertyNameAttribute.PropertyType.Range)]
	public string StormIntensityParameter = null;
	
	public float MaxLightAlpha = 0.8f;
	public float MinDetailAlpha = 0.3f;

		// Use this for initialization
	void Start () 
	{
		m_timeOfDay = FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		m_weather 	= FindObjectOfType(typeof(Weather)) as Weather;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector4 color = m_timeOfDay.TODColor;
		color.w = Mathf.Min(m_timeOfDay.TODColorMagnitude, MaxLightAlpha);
		
		renderer.material.color = color;
		
		if(!string.IsNullOrEmpty(StormIntensityParameter))
		{
			float intensity = Mathf.Lerp(MinDetailAlpha, 1.0f, m_weather.StormIntensity);
			renderer.material.SetFloat(StormIntensityParameter, intensity);	
		}
	}
	
	private TimeOfDay m_timeOfDay 	= null;
	private Weather m_weather 		= null;
}
