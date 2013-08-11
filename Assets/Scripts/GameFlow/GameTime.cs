///////////////////////////////////////////////////////////
// 
// GameTime.cs
//
// What it does: Keeps track of an independent time to that being used by Unity.
//				 Handy for pausing gameplay without bollocking up the game globally.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class GameTime
{
	public event System.Action TimePaused;
	public event System.Action TimeUnpaused;
	
	public static GameTime Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new GameTime();
			}
			
			return s_instance;
		}
	}
	
	public static float DeltaTime
	{
		get { return Instance.m_deltaTime; }
	}
	
	public void Update()
	{
		if(!Paused)
		{
			m_deltaTime = Time.deltaTime;
		}
	}
	
	public bool Paused 
	{ 
		get
		{
			return m_paused;	
		}
		
		set
		{
			bool paused = value;
			
			if(paused != m_paused)
			{
				m_paused = paused; //LHS
				
				if(m_paused && TimePaused != null)
				{
					TimePaused();	
				}
				else if(!m_paused && TimeUnpaused != null)
				{
					TimeUnpaused();
				}
			}
		}
	}
	
	private GameTime() { }
	
	public float m_deltaTime			= 0.0f;
	private static GameTime s_instance 	= null;
	private bool m_paused 				= false;
	
}
