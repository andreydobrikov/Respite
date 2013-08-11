///////////////////////////////////////////////////////////
// 
// DisableScriptOnPause.cs
//
// What it does: Take a fucking guess
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class DisableScriptOnPause : MonoBehaviour 
{
	public Component TargetComponent = null;
	
	void Start () 
    {
		GameTime.Instance.TimePaused 	+= new System.Action(OnPause);	
		GameTime.Instance.TimeUnpaused 	+= new System.Action(OnUnpause);
	}

	private void OnPause()
	{
		if(TargetComponent != null)
		{
			MonoBehaviour tempBehaviour = TargetComponent as MonoBehaviour;
			if(tempBehaviour != null)
			{
				tempBehaviour.enabled = false;
			}
		}
	}
	
	private void OnUnpause() 
	{
		if(TargetComponent != null)
		{
			MonoBehaviour tempBehaviour = TargetComponent as MonoBehaviour;
			if(tempBehaviour != null)
			{
				tempBehaviour.enabled = true;
			}
		}
	}
}

