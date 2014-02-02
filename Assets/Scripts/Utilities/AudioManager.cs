///////////////////////////////////////////////////////////
// 
// AudioManager.cs
//
// What it does: Tracks settings, updates global audio states.
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
    public GameObject Player                = null;
	public AudioSource WeatherAudioSource   = null;

	void Start()
	{
		if(WeatherAudioSource == null) { Debug.LogWarning("<color=orange>AudioManager: WeatherAudioSource not set!</color>"); }
        if(Player == null) { Debug.LogWarning("<color=orange>AudioManager: Player not set!</color>"); }
	}

	void Update()
	{
		UpdateSettings(); 			// Update the audio-system according to user-settings.
		UpdateWeatherTransitions();	// Update weather-audio settings according to occupied transition-zones.

        WeatherAudioSource.pitch = Mathf.Lerp(WeatherAudioSource.pitch, m_targetWeatherPitch, 0.02f);
	}

	// Registers a transition-zone to determine weather-effect settings.
	public void RegisterWeatherTransitionZone(WeatherAudioTransitionZone zone)
	{
		m_activeTransitionZones.Add(zone);
	}

	// Unregisters a transition-zone.
	public void UnregisterWeatherTransitionZone(WeatherAudioTransitionZone zone)
	{
		m_activeTransitionZones.Remove(zone);
	}

	#region Update Functions

	// Update audio settings in accordance with global audio values
	private void UpdateSettings()
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

	// Update weather-audio settings according to occupied transition-zones.
	private void UpdateWeatherTransitions()
	{
		if(m_activeTransitionZones.Count == 0 || 
           WeatherAudioSource == null || 
           Player == null)
		{
			return;
		}

        float newPitch = 0.0f;

        // Calculate the new pitches and use the highest.
        foreach(var transitionZone in m_activeTransitionZones)
        {
            newPitch = Mathf.Max(transitionZone.GetPitch(Player.transform.position), newPitch);
        }

        m_targetWeatherPitch = newPitch;
        
	}

	#endregion

	List<WeatherAudioTransitionZone> m_activeTransitionZones = new List<WeatherAudioTransitionZone>(); // List of transition zones to update.
    float m_targetWeatherPitch = 0.0f;
}
