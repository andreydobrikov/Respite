using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Interactive Objects/Door")]
public class Door : InteractiveObject 
{
	public GameObject m_targetObject = null;
	public float openRate = 0.02f;
	public float maxRotation = -65.0f;
	
	public Door()
	{
		m_openInteraction 	= new Interaction("Open", new Interaction.InteractionHandler(HandleOpen), ContextFlag.World);
		m_closeInteraction 	= new Interaction("Close", new Interaction.InteractionHandler(HandleClose), ContextFlag.World);
		
		m_closeInteraction.Enabled = false;
		
		m_interactions.Add(m_openInteraction);
		m_interactions.Add(m_closeInteraction);
	}

	
	public void Start()
	{
		m_initialRotation = transform.rotation;	
	}
	
	public void FixedUpdate()
	{
		m_lerpProgress += (m_lerpDirection * openRate);
		m_lerpProgress = Mathf.Max(m_lerpProgress, 0.0f);
		m_lerpProgress = Mathf.Min(m_lerpProgress, 1.0f);
		
		m_currentRotation = (maxRotation * Mathf.Sin(m_lerpProgress * Mathf.PI / 2.0f)) ;
		
		m_targetObject.GetComponent<Rigidbody>().MoveRotation(m_initialRotation * Quaternion.Euler(new Vector3(0.0f, m_currentRotation, 0.0f)));	
	}
		
	private void HandleOpen(Interaction interaction)
	{
		m_lerpDirection = 1.0f;
		
		m_openInteraction.Enabled = false;
		m_closeInteraction.Enabled = true;
		
		AudioSource source = GetComponent<AudioSource>() as AudioSource;
		
		if(source != null)
		{
			source.Play();
		}
	}
	
	private void HandleClose(Interaction interaction)
	{
		m_lerpDirection = -1.0f;
		
		m_closeInteraction.Enabled 	= false;
		m_openInteraction.Enabled 	= true;
		
		AudioSource source = GetComponent<AudioSource>() as AudioSource;
		
		source.Play();
	}
	
	public override void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("current_rotation", 	m_currentRotation.ToString()));
		pairs.Add(new SavePair("lerp_direction", 	m_lerpDirection.ToString()));
		pairs.Add(new SavePair("lerp_progress", 	m_lerpProgress.ToString()));
	}
	
	public override void SaveDeserialise(List<SavePair> pairs)
	{
		foreach(var pair in pairs)
		{
			if(pair.id == "current_rotation") {	float.TryParse(pair.value, out m_currentRotation); }
			if(pair.id == "lerp_direction") {	float.TryParse(pair.value, out m_lerpDirection); }
			if(pair.id == "lerp_progress") {	float.TryParse(pair.value, out m_lerpProgress); }
			
			Debug.Log("Door deserialising value " + pair.value);	
		}
		
		m_openInteraction.Enabled 	= m_lerpDirection <= 0.0f;
		m_closeInteraction.Enabled 	= m_lerpDirection > 0.0f;
	}
	
	private Interaction m_openInteraction 	= null;
	private Interaction m_closeInteraction 	= null;
	private float m_currentRotation = 0.0f;
	private float m_lerpProgress = 0.0f;
	private float m_lerpDirection = -1.0f;
	private Quaternion m_initialRotation = Quaternion.identity;
}
