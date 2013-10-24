///////////////////////////////////////////////////////////
// 
// ToggleObject.cs
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

public class ToggleObject : InteractiveObject 
{
	public string ToggleOnString = "Toggle On";
	public string ToggleOffString = "Toggle Off";
	
	public List<ToggleableObject> ToggleTargets = new List<ToggleableObject>();
	
	public bool StartingOn = true;
	
	public ToggleObject()
	{
		m_toggleOnInteraction = new Interaction(ToggleOnString, ToggleOn);
		m_toggleOffInteraction = new Interaction(ToggleOffString, ToggleOff);
		
		m_interactions.Add(m_toggleOnInteraction);
		m_interactions.Add(m_toggleOffInteraction);
	}
	
	void Start()
	{
		if(StartingOn)
		{
			ToggleOn(null, null);	
		}
		else
		{
			ToggleOff(null, null);	
		}
	}
	
	private void ToggleOn(Interaction source, GameObject trigger)
	{
		m_toggleOffInteraction.Enabled 	= true;
		m_toggleOnInteraction.Enabled 	= false;
		
		foreach(var toggle in ToggleTargets)
		{
			toggle.ToggleOn();		
		}
	}
	
	private void ToggleOff(Interaction source, GameObject trigger)
	{
		m_toggleOffInteraction.Enabled 	= false;
		m_toggleOnInteraction.Enabled 	= true;
		
		foreach(var toggle in ToggleTargets)
		{
			toggle.ToggleOff();		
		}
	}
	
	private Interaction m_toggleOnInteraction = null;
	private Interaction m_toggleOffInteraction = null;
}
