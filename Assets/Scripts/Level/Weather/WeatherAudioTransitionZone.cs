/// <summary>
/// Weather audio transition zone.
/// 
/// This behaviour updates the AudioManager with transition zones the player occupies.
/// The AudioManager is responsible for actually update pitches and the like.
/// This is mainly to avoid having loads of zones updating redundantly when the player is outside their range.
/// </summary>

using UnityEngine;
using System.Collections;

public class WeatherAudioTransitionZone : MonoBehaviour 
{
    public Vector3 TransitionLocus  = Vector3.zero;
    public float TargetPitch        = 0.0f;
	public float TargetVolume		= 0.0f;
    public float TransitionRadius   = 1.0f;
    public Door DoorModifier        = null;

	// Find the instance of the AudioManager and cache it for later.
	void Start () 
	{
		m_audioManager = FindObjectOfType<AudioManager>();

		FloatSetting pitchSetting = Settings.Instance.GetSetting<FloatSetting>("audio_outdoor_pitch");
		FloatSetting volumeSetting = Settings.Instance.GetSetting<FloatSetting>("audio_outdoor_volume");
        
		if(pitchSetting != null)	{ m_outdoorDefaultPitch = pitchSetting.Value; }
		if (volumeSetting != null)	{ m_outdoorDefaultVolume = volumeSetting.Value; }
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player" && m_audioManager != null)
		{
			m_audioManager.RegisterWeatherTransitionZone(this);
			Debug.Log("Transition Zone Entered");
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player" && m_audioManager != null)
		{
			m_audioManager.UnregisterWeatherTransitionZone(this);
			Debug.Log("Transition Zone Exited");
		}
	}

    public void GetPitchAndVolume(Vector3 targetPosition, out float pitch, out float volume)
    {
        // TODO: Pre-square the range if this proves slow. I seriously doubt it will, though.
        float lerpFactor = ((TransitionLocus + transform.position) - targetPosition).magnitude;
        lerpFactor = Mathf.Min(lerpFactor, TransitionRadius);
        lerpFactor = lerpFactor / TransitionRadius;

        if(DoorModifier != null)
        {
            if(DoorModifier.State == Door.DoorState.Closed)
            {
                // TODO: Scale this against the door's closing progress instead of setting the value.
                lerpFactor = 1.0f;
            }
        }

        pitch = Mathf.Lerp(m_outdoorDefaultPitch, TargetPitch, lerpFactor);
		volume = Mathf.Lerp(m_outdoorDefaultVolume, TargetVolume, lerpFactor);
    }

	private AudioManager m_audioManager = null;

    private float m_outdoorDefaultPitch		= 0.0f;
	private float m_outdoorDefaultVolume	= 0.0f;
}
