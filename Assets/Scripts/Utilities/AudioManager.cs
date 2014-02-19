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
		if(WeatherAudioSource == null) { Logger.LogWarning("AudioManager: WeatherAudioSource not set!", LogChannel.Audio); }
        if(Player == null) { Logger.LogWarning("AudioManager: Player not set!", LogChannel.Audio); }

		FloatSetting weatherLerpSpeedSetting = Settings.Instance.GetSetting<FloatSetting>("audio_weather_lerp_speed");

		if (weatherLerpSpeedSetting != null) 
		{
			weatherLerpSpeedSetting.AddChangedCallback(WeatherLerpSpeedSettingChanged);
			m_weatherLerpSpeed = weatherLerpSpeedSetting.Value; 
		}
	}

	void Update()
	{
		UpdateSettings(); 			// Update the audio-system according to user-settings.
		UpdateWeatherTransitions();	// Update weather-audio settings according to occupied transition-zones.

		WeatherAudioSource.pitch = Mathf.Lerp(WeatherAudioSource.pitch, m_targetWeatherPitch, m_weatherLerpSpeed);
		WeatherAudioSource.volume = Mathf.Lerp(WeatherAudioSource.volume, m_targetWeatherPitch, m_weatherLerpSpeed);
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
		BoolSetting audioEnabled = Settings.Instance.GetSetting<BoolSetting>("audio_enabled");
		
		if(audioEnabled != null &&  audioEnabled.Value)
		{	
			AudioListener.pause = false;	
		}
		else 
		{
			AudioListener.pause = true;
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
		float newVolume = 0.0f;

        // Calculate the new pitches and use the highest.
        foreach(var transitionZone in m_activeTransitionZones)
        {
			float zonePitch = 0.0f;
			float zoneVolume = 0.0f;
			
			transitionZone.GetPitchAndVolume(Player.transform.position, out zonePitch, out zoneVolume);

            newPitch	= Mathf.Max(zonePitch, newPitch);
			newVolume	= Mathf.Max(zoneVolume, newVolume);
        }

        m_targetWeatherPitch = newPitch;

		Logger.Log("New pitch: " + newPitch, LogChannel.Audio);
		Logger.Log("New volume: " + newVolume, LogChannel.Audio);
	}

	#endregion

#if UNITY_EDITOR
	public void WeatherLerpSpeedSettingChanged()
	{
		FloatSetting weatherLerpSpeedSetting = Settings.Instance.GetSetting<FloatSetting>("audio_weather_lerp_speed");
		if (weatherLerpSpeedSetting != null) { m_weatherLerpSpeed = weatherLerpSpeedSetting.Value; }
	}

#endif

	List<WeatherAudioTransitionZone> m_activeTransitionZones = new List<WeatherAudioTransitionZone>(); // List of transition zones to update.

    float m_targetWeatherPitch = 0.0f;
	float m_targetWeatherVolume = 0.0f;

	float m_weatherLerpSpeed = 0.2f;
}
