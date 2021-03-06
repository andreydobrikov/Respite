///////////////////////////////////////////////////////////
// 
// TransitionZone.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class TransitionZone : MonoBehaviour 
{
	public Building LightsDisableObject = null;
	public Building LightsEnableObject = null;
	
	public GameObject TeleportTarget = null;
	
	public float HeightOffset = 0.0f;
	public float TransitionDuration = 0.6f;
	
	void Start()
	{
		m_fade = GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			m_other = other;
			m_fade.StartFade(Color.black, TransitionDuration / 2.0f, FadeComplete);
		}
	}
				
	private void FadeComplete()
	{
		if(TeleportTarget != null)
		{
			m_other.rigidbody.Sleep();
			m_other.rigidbody.position = TeleportTarget.transform.position;
			m_other.rigidbody.WakeUp();
		}
		
		if(m_other != null)
		{
			m_other.BroadcastMessage("OnRegionTransition", SendMessageOptions.DontRequireReceiver);
			m_other.rigidbody.position = m_other.rigidbody.position + new Vector3(0.0f, HeightOffset, 0.0f);
		}
		m_other = null;
		
		// Lightmaps have to be disabled to avoid confusion, so prompt the floors for that
		LightsDisableObject.DisableLights();
		LightsEnableObject.EnableLights();
		
		Debug.Log("Transitioned");
		m_fade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), TransitionDuration / 2.0f, null);
	}
	
	private CameraFade m_fade 	= null;
	private Collider m_other 	= null;
}
