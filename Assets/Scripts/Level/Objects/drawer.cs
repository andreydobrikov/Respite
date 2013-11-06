///////////////////////////////////////////////////////////
// 
// drawer.cs
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

[RequireComponent(typeof(Rigidbody))]
public class drawer : InteractiveObject 
{
	public float OpenDistance = 1.0f;
	public float SpeedMultiplier = 1.0f;
	public ObjectPool m_noisePool;
	
	public drawer()
	{
		m_openInteraction 	= new Interaction("Open", new Interaction.InteractionHandler(HandleOpen), ContextFlag.World);	
		m_closeInteraction 	= new Interaction("Close", new Interaction.InteractionHandler(HandleClose), ContextFlag.World);	
		
		m_closeInteraction.Enabled = false;
		
		m_interactions.Add(m_openInteraction);
		m_interactions.Add(m_closeInteraction);
	}

	void Start ()
	{
		m_body = GetComponent<Rigidbody>();
		m_startPosition = m_body.position;
		m_openDirection = transform.rotation * new Vector3(OpenDistance, 0.0f, 0.0f);
	}

	void Update ()
	{
#if UNITY_EDITOR
		m_openDirection = transform.rotation * new Vector3(OpenDistance, 0.0f, 0.0f);
#endif
		
		if(m_opening)
		{
			m_openProgress += Time.deltaTime * SpeedMultiplier;	
		}
		else
		{
			m_openProgress -= Time.deltaTime * SpeedMultiplier;	
		}
		
		m_openProgress = Mathf.Clamp(m_openProgress, 0.0f, 1.0f);
		
		
		
		m_body.position = Vector3.Lerp(m_startPosition, m_startPosition + m_openDirection, m_openProgress);
			
	}
	
	private void HandleOpen(Interaction interaction, GameObject trigger)
	{
		Debug.Log("Drawer Opened");	
		m_opening = true;
		
		m_openInteraction.Enabled = false;
		m_closeInteraction.Enabled = true;
		
		if(m_noisePool != null)
		{
			GameObject noiseRipple = m_noisePool.ActivateObject();	
			noiseRipple.transform.position = transform.position;
		}
	}
	
	private void HandleClose(Interaction interaction, GameObject trigger)
	{
		Debug.Log("Drawer Closed");	
		m_opening = false;
		
		m_openInteraction.Enabled = true;
		m_closeInteraction.Enabled = false;
		
		if(m_noisePool != null)
		{
			GameObject noiseRipple = m_noisePool.ActivateObject();	
			noiseRipple.transform.position = transform.position;
		}
	}
	
	private Vector3 m_openDirection;
	private bool m_opening = false;
	private float m_openProgress = 0.0f;
	private Interaction m_openInteraction;
	private Interaction m_closeInteraction;
	private Vector3 m_startPosition;
	private Rigidbody m_body;
}
