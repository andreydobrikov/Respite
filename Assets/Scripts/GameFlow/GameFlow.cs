///////////////////////////////////////////////////////////
// 
// GameFlow.cs
//
// What it does: Singleton controlling all changes between states of the game.
//
// Notes: 	Save/Load
//			Inventory
//			Inspection
//			Menus
//			
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class GameFlow
{
	public enum ControlContext
	{
		World,
		Inspection,
		Inventory,
		Menu,
		GameOver,
		Loading
	}
	
	public float SaveFadeDuration = 3.0f;
	public float GameDuration = 0.0f;
	
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
	
	public void LevelLoaded()
	{
		m_gameTimer = 0.0f;
		m_context.Clear();

		// When starting the level, always give control to the player
		m_context.Push(ControlContext.World);

		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.5f, null);
		m_postManager	= GameObject.FindObjectOfType(typeof(PostProcessManager)) as PostProcessManager;
		m_timeOfDay 	= GameObject.FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		m_cameraFade 	= GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;	
		m_gameTime 		= GameTime.Instance;

		Time.timeScale = 1.0f;
	}

	public void Update()
	{
		GameTime.Instance.Update();

		m_gameTimer += GameTime.Instance.m_deltaTime;

		// Don't check for game-over if the game is over!
		if(m_context.Peek() != ControlContext.GameOver)
		{
			if(m_gameTimer >= GameDuration)
			{
				GameOver();
			}
		}


		// Check to see if the level is still reloading
		if(m_loadOperation != null)
		{
			Debug.Log(m_loadOperation.progress);
			if(m_loadOperation.isDone)
			{
				// Groovy, all done. Fade up and return control to the world
				m_loadOperation = null;
				m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.5f, LevelLoadFadeComplete);
				m_context.Clear();
				m_context.Push(ControlContext.World);
			}
		}
	}
	
	public void RequestSave(float advanceTime)
	{
		m_advanceTime = advanceTime;
		
		if(m_cameraFade != null)
		{
			Debug.Log("Fading down");
			m_cameraFade.StartFade(Color.black, SaveFadeDuration, SaveScreenFadeComplete);
		}
	}
	
	public void RequestLoad()
	{
		if(m_cameraFade != null)
		{
			Debug.Log("Fading down");
			m_cameraFade.StartFade(Color.black, SaveFadeDuration, LoadScreenFadeComplete);
		}	
	}
	
	public void RequestInventory()
	{
		if(m_postManager != null)
		{
			m_postManager.ActivateBlur();
		}
		
		m_context.Push(ControlContext.Inventory);	
	}
	
	public void EndInventory()
	{
		if(m_postManager != null)
		{
			m_postManager.DeactivateBlur();
		}
		
		m_context.Pop();	
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
		if(m_cameraFade != null)
		{
			m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f, MenuFadeComplete);
		}
		
		m_gameTime.Paused = true;
		Time.timeScale = 0.0f;
	}
	
	public void EndMenu()
	{
		if(m_cameraFade != null)
		{
			m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.5f, null);
		}
		
		m_context.Pop();
		
		m_gameTime.Paused = false;
		Time.timeScale = 1.0f;
	}
	
	public void GameOver()
	{
		Time.timeScale = 0.0f;
		if(m_cameraFade != null)
		{
			m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f, GameOverFadeComplete);
		}	
	}
	
	public ControlContext CurrentControlContext
	{
		get
		{
			return m_context.Peek();	
		}
	}
	
	public float LoadProgress { get { return m_loadOperation != null ? m_loadOperation.progress : 1.0f; } }

	public float GameTimerProgress { get { return m_gameTimer; } }
	
	public void ResetLevel()
	{
		m_context.Push(ControlContext.Loading);
		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f, null);
		m_loadOperation = Application.LoadLevelAsync("AgentView");	
	}
	
	private GameFlow()
	{
		m_postManager	= GameObject.FindObjectOfType(typeof(PostProcessManager)) as PostProcessManager;
		m_timeOfDay 	= GameObject.FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		m_cameraFade 	= GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;	
		
		m_gameTime 		= GameTime.Instance;
		
		m_context.Push(ControlContext.World);
	}
	
	private void SaveScreenFadeComplete()
	{
		Serialiser.Instance.Serialise();
		
		m_timeOfDay.AdjustedTime = m_timeOfDay.AdjustedTime + m_advanceTime;
		
		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), SaveFadeDuration, null);
	}
	
	private void LoadScreenFadeComplete()
	{
		Serialiser.Instance.Deserialise();
		
		m_cameraFade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), SaveFadeDuration, null);
	}
	
	private void MenuFadeComplete()
	{
		m_context.Push(ControlContext.Menu);
	}
	
	private void GameOverFadeComplete()
	{
		m_context.Push(ControlContext.GameOver);
		
	}
	
	private void LevelLoadFadeComplete()
	{
		
	}
	
	private GameTime			m_gameTime		= null;
	private PostProcessManager 	m_postManager	= null;
	private TimeOfDay			m_timeOfDay		= null;
	private CameraFade 			m_cameraFade 	= null;
	private float				m_advanceTime 	= 0.0f;
	private AsyncOperation		m_loadOperation = null;
	private float 				m_gameTimer 	= 0.0f;
	
	private Stack<ControlContext> m_context = new Stack<ControlContext>();
}
