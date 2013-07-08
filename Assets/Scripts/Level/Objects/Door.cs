using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Interactive Objects/Door")]
public class Door : InteractiveObject 
{
	public GameObject m_targetObject = null;
	public float openRate = 0.02f;
	
	public Door()
	{
		m_openInteraction 	= new Interaction("Open", new Interaction.InteractionHandler(HandleOpen));
		m_closeInteraction 	= new Interaction("Close", new Interaction.InteractionHandler(HandleClose));
		
		m_closeInteraction.Enabled = false;
		
		m_interactions.Add(m_openInteraction);
		m_interactions.Add(m_closeInteraction);
	}
	
	public override List<Interaction> GetInteractions()
	{
		return m_interactions;
	}
	
	public void FixedUpdate()
	{
		m_lerpProgress += (m_lerpDirection * openRate);
		m_lerpProgress = Mathf.Max(m_lerpProgress, 0.0f);
		m_lerpProgress = Mathf.Min(m_lerpProgress, 1.0f);
		
		m_currentRotation = (m_openRotation * Mathf.Sin(m_lerpProgress * Mathf.PI / 2.0f)) ;
		
		m_targetObject.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(new Vector3(0.0f, 0.0f, m_currentRotation)));	
	}
		
	private void HandleOpen(Interaction interaction)
	{
		m_lerpDirection = 1.0f;
		
		m_openInteraction.Enabled = false;
		m_closeInteraction.Enabled = true;
	}
	
	private void HandleClose(Interaction interaction)
	{
		m_lerpDirection = -1.0f;
		
		m_closeInteraction.Enabled 	= false;
		m_openInteraction.Enabled 	= true;
	}
	
	private List<Interaction> m_interactions = new List<Interaction>();
	
	private Interaction m_openInteraction 	= null;
	private Interaction m_closeInteraction 	= null;
	private float m_openRotation = -65.0f;
	private float m_currentRotation = 0.0f;
	private float m_lerpProgress = 0.0f;
	private float m_lerpDirection = -1.0f;
}
