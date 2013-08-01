using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Vignetting))]
public class WeatherVignette : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	//	m_vignetting = GetComponent<Vignetting>();
		m_weather	= GameObject.FindObjectOfType(typeof(Weather)) as Weather;
	}
	
	// Update is called once per frame
	void Update () 
	{
	//	m_vignetting.intensity = m_weather.StormIntensity;
	//	m_vignetting.blur = m_weather.StormIntensity;
	//	m_vignetting.blurSpread = m_weather.StormIntensity * 2.0f;
	}
	
	//private Vignetting m_vignetting = null;
	private Weather m_weather		= null;
}
