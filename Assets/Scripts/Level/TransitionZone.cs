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
	public GameObject DisableObject = null;
	public GameObject EnableObject = null;
	
	public float HeightOffset = -1.0f;
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
		if(m_other != null)
		{
			m_other.BroadcastMessage("OnRegionTransition", SendMessageOptions.DontRequireReceiver);
		}
		m_other = null;
		
		DisableObject.SetActive(false);
		EnableObject.SetActive(true);
		
		Debug.Log("Transitioned");
		m_fade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), TransitionDuration / 2.0f, null);
	}
	
	private CameraFade m_fade = null;
	private Collider m_other = null;
}
