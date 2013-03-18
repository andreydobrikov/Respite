using UnityEngine;
using System.Collections;

public class AdminResources : MonoBehaviour 
{
	public float StartResources = 0.0f;
	public float MaxResources = 100.0f;
	public float RechargeRate = 0.1f;
	
	// Use this for initialization
	void Start () 
	{
		m_currentResources = StartResources;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(m_currentResources < MaxResources)
		{
			m_currentResources += RechargeRate;	
		}
	}
	
	void OnGUI()
	{
		GUI.TextArea(new Rect(Screen.width / 2 - 100, 0, 200, 80), "Resource: " + (int)m_currentResources);
	}
	
	private float m_currentResources = 0.0f;
}
