///////////////////////////////////////////////////////////
// 
// AudioManager.cs
//
// What it does: Just a helper to track the global audio settings.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
	void Update()
	{
		// Grab the setting and enabled or disable the audio-listener as appropriate.
		string globalAudioToggle = Settings.Instance.GetSetting("audio_enabled");
		
		if(	globalAudioToggle != null && globalAudioToggle == "false")
		{	
			AudioListener.pause = true;	
		}
		else if(globalAudioToggle == "true")
		{
			AudioListener.pause = false;
		}
	}
	
}
