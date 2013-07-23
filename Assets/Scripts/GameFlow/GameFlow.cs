using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlow
{
	public float SaveFadeDuration = 3.0f;
	
	private static GameFlow m_instance = null;	
	
	public static GameFlow Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new GameFlow();
			}
			
			return m_instance;
		}
	}
	
	public void Update()
	{
	}
	
	public void RequestSave(float advanceTime)
	{
		m_advanceTime = advanceTime;
		
		if(m_cameraFade != null)
		{
			Debug.Log("Fading down");
			m_cameraFade.StartFade(Color.black, SaveFadeDuration, ScreenFadeComplete);
		}
	}
	
	private GameFlow()
	{
		m_timeOfDay 	= GameObject.FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		m_cameraFade 	= GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;	
	}
	
	private void ScreenFadeComplete()
	{
		m_timeOfDay.AdjustedTime = m_timeOfDay.AdjustedTime + m_advanceTime;
		
		Serialiser.Instance.Serialise();
		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), SaveFadeDuration, null);
	}
	
	private TimeOfDay	m_timeOfDay		= null;
	private CameraFade 	m_cameraFade 	= null;
	private float		m_advanceTime 	= 0.0f;
}
