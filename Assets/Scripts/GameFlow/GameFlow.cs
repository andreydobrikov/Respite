using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlow
{
	public enum ControlContext
	{
		World,
		Inspection,
		Menu
	}
	
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
		GameTime.Instance.Update();
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
	
	public void RequestInspection()
	{
		if(m_postManager != null)
		{
			m_postManager.ActivateBlur();
		}
		
		m_context.Push(ControlContext.Inspection);
	}
	
	public void EndInspection()
	{
		if(m_postManager != null)
		{
			m_postManager.DeactivateBlur();
		}
		
		m_context.Pop();
	}
	
	public void RequestMenu()
	{
		if(m_postManager != null)
		{
			m_postManager.ActivateBlur();
		}
		
		m_context.Push(ControlContext.Menu);
		
		m_gameTime.Paused = true;
	}
	
	public void EndMenu()
	{
		if(m_postManager != null)
		{
			m_postManager.DeactivateBlur();
		}
		
		m_context.Pop();
		
		m_gameTime.Paused = false;
	}
	
	public ControlContext CurrentControlContext
	{
		get
		{
			return m_context.Peek();	
		}
	}
	
	private GameFlow()
	{
		m_postManager	= GameObject.FindObjectOfType(typeof(PostProcessManager)) as PostProcessManager;
		m_timeOfDay 	= GameObject.FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		m_cameraFade 	= GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;	
		
		m_gameTime 		= GameTime.Instance;
		
		m_context.Push(ControlContext.World);
	}
	
	private void ScreenFadeComplete()
	{
		m_timeOfDay.AdjustedTime = m_timeOfDay.AdjustedTime + m_advanceTime;
		
		Serialiser.Instance.Serialise();
		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), SaveFadeDuration, null);
	}
	
	private GameTime			m_gameTime		= null;
	private PostProcessManager 	m_postManager	= null;
	private TimeOfDay			m_timeOfDay		= null;
	private CameraFade 			m_cameraFade 	= null;
	private float				m_advanceTime 	= 0.0f;
	
	private Stack<ControlContext> m_context = new Stack<ControlContext>();
}
