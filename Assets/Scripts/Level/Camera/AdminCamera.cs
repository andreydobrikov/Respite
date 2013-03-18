using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class AdminCamera : WorldViewObject  
{
	MeshRenderer m_renderer = null;
	Camera m_camera = null;

	// Use this for initialization
	void Start () 
	{
		m_renderer = GetComponent<MeshRenderer>();
		m_camera = m_worldObject as Camera;
		m_camera.StateChanged += new System.EventHandler(OnStateChanged);
		
	}
	
	void FixedUpdate()
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void OnStateChanged(object sender, System.EventArgs args)
	{
		Debug.Log("State changed");
		if(m_camera.GetState() == Camera.TargetState.Spotted)
		{
				m_renderer.enabled = false;
		}
		else
		{
			m_renderer.enabled = true;	
		}
	}
	
	void OnGUI()
	{
		
	}
}
