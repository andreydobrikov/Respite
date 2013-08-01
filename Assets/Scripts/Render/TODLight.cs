using UnityEngine;
using System.Collections;

public class TODLight : MonoBehaviour 
{
	[ShaderPropertyNameAttribute(ShaderPropertyNameAttribute.PropertyType.Range)]
	public string StormIntensityParameter = null;
	
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
		color.w = Mathf.Min(((Vector3)color).magnitude, 0.8f);
		
		renderer.material.color = color;
		
		if(!string.IsNullOrEmpty(StormIntensityParameter))
		{
			renderer.material.SetFloat(StormIntensityParameter, 1.0f - m_weather.StormIntensity);	
		}
	}
	
	private TimeOfDay m_timeOfDay 	= null;
	private Weather m_weather 		= null;
}
