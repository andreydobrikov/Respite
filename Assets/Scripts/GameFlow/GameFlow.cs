using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlow
{
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
}
