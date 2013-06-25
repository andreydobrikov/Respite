using UnityEngine;
using System.Collections;

public class TODLight : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		m_timeOfDay = FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector4 color = m_timeOfDay.TODColor;
		color.w = Mathf.Min(((Vector3)color).magnitude, 0.8f);
		
		renderer.material.color = color;
	}
	
	private TimeOfDay m_timeOfDay = null;
}
