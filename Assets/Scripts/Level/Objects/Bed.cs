using UnityEngine;
using System.Collections.Generic;

public class Bed : InteractiveObject 
{
	void Start()
	{
		m_sleepInteraction = new Interaction("Sleep", OnSleepInteraction);
		
		m_interactions.Add(m_sleepInteraction);
	}
		
	private void OnSleepInteraction(Interaction interaction)
	{
		GameFlow.Instance.RequestSave(0.3f);
	}
	
	Interaction m_sleepInteraction = null;
}
