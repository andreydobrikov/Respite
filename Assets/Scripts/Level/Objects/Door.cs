using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Interactive Objects/Door")]
public class Door : InteractiveObject 
{
	public ObjectPool m_noisePool;
	public GameObject m_targetObject = null;
	public float openRate = 0.02f;
	public float maxRotation = -65.0f;
	public bool Locked = false;
	public NavMeshObstacle ObstacleObject = null;
	
	public enum DoorState
	{
		Closed,
		Opening,
		Open,
		Closing
	}
	
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
		
		if(Locked)
		{
			Lock();
		}

		
		if(ObstacleObject != null)
		{
			ObstacleObject.enabled = false;
		}
	}
	
	public void FixedUpdate()
	{
		m_lerpProgress += (m_lerpDirection * openRate);
		
		if(m_lerpDirection > 0.0f)
		{
			m_lerpProgress = Mathf.Max(m_lerpProgress, 0.0f);
			m_lerpProgress = Mathf.Min(m_lerpProgress, m_targetValue);
		}
		else
		{
			m_lerpProgress = Mathf.Max(m_lerpProgress, m_targetValue);
			m_lerpProgress = Mathf.Min(m_lerpProgress, 1.0f);
		}
		
		m_currentRotation = (maxRotation * Mathf.Sin(m_lerpProgress * Mathf.PI / 2.0f)) ;
		
		m_targetObject.GetComponent<Rigidbody>().MoveRotation(m_initialRotation * Quaternion.Euler(new Vector3(0.0f, m_currentRotation, 0.0f)));	
		
		if(Mathf.Abs(m_lerpProgress) == 1.0f)
		{

			if(ObstacleObject != null) ObstacleObject.enabled = true;
			m_doorState = DoorState.Open;	
		}
		else if(m_lerpProgress == 0.0f)
		{
			if(ObstacleObject != null) ObstacleObject.enabled = false;
			m_doorState = DoorState.Closed;	
		}
		else if(m_lerpDirection > 0.0f)
		{
			if(ObstacleObject != null) ObstacleObject.enabled = true;
			m_doorState = DoorState.Opening;	
		}
		else
		{
			m_doorState = DoorState.Closing;	
		}
	}
	
	public void Open(GameObject trigger)
	{
		Vector2 posXY = new Vector2(trigger.transform.position.x, trigger.transform.position.z);
		
		Vector2 hingeStart = new Vector2(PivotPosition.x, PivotPosition.z);
		Vector2 hingeEnd = hingeStart + new Vector2(DoorDirection.x, DoorDirection.z);
		
		float sign = MathsHelper.sign(posXY, hingeStart, hingeEnd);
		
		sign = sign / Mathf.Abs(sign);
		m_lerpDirection = -sign;// 1.0f;	
		
		m_openInteraction.Enabled = false;
		m_closeInteraction.Enabled = true;
		
		m_targetValue = -sign;
		
		AudioSource source = GetComponent<AudioSource>() as AudioSource;
		
		if(source != null)
		{
			source.Play();
		}
	}
	
	public void Lock()
	{
		m_openInteraction.Enabled 	= false;
		m_closeInteraction.Enabled 	= false;
		
		m_currentRotation = 0.0f;
		m_lerpProgress = 0.0f;
		m_lerpDirection = -1.0f;
		m_targetValue = 0.0f;
	}
	
	public void Unlock()
	{
		m_openInteraction.Enabled = true;
	}
		
	private void HandleOpen(Interaction interaction, GameObject trigger)
	{
		if(m_noisePool != null)
		{
			GameObject noiseRipple = m_noisePool.ActivateObject();	
			noiseRipple.transform.position = transform.position;
		}
		
		Open(trigger);
	}
	
	private void HandleClose(Interaction interaction, GameObject trigger)
	{
		if(m_noisePool != null)
		{
			GameObject noiseRipple = m_noisePool.ActivateObject();	
			noiseRipple.transform.position = transform.position;
		}
		
		m_lerpDirection = -1.0f;
		
		m_closeInteraction.Enabled 	= false;
		m_openInteraction.Enabled 	= true;
		
		m_targetValue = 0.0f;
		
		AudioSource source = GetComponent<AudioSource>() as AudioSource;
		if(source != null)
		{
			source.Play();
		}
	}
	
	public override void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("current_rotation", 	m_currentRotation.ToString()));
		pairs.Add(new SavePair("lerp_direction", 	m_lerpDirection.ToString()));
		pairs.Add(new SavePair("lerp_progress", 	m_lerpProgress.ToString()));
		pairs.Add(new SavePair("target_value", 		m_targetValue.ToString()));
	}
	
	public override void SaveDeserialise(List<SavePair> pairs)
	{
		foreach(var pair in pairs)
		{
			if(pair.id == "current_rotation") 	{	if(!float.TryParse(pair.value, out m_currentRotation)) { Debug.LogError("BLURP"); }; }
			if(pair.id == "lerp_direction") 	{	if(!float.TryParse(pair.value, out m_lerpDirection)) { Debug.LogError("BLURP"); }; }
			if(pair.id == "lerp_progress") 		{	if(!float.TryParse(pair.value, out m_lerpProgress)) { Debug.LogError("BLURP"); }; }
			if(pair.id == "target_value") 		{	if(!float.TryParse(pair.value, out m_targetValue)) { Debug.LogError("BLURP"); }; }
			
			Debug.Log("Door deserialising key " + pair.id);	
			Debug.Log("Door deserialising value " + pair.value);	
		}
		
		m_openInteraction.Enabled 	= m_lerpDirection <= 0.0f;
		m_closeInteraction.Enabled 	= m_lerpDirection > 0.0f;
	}
	
	public DoorState State
	{
		get { return m_doorState; }	
	}
	
    public Vector3 DoorDirection
    {
		get 
		{
			// Grab the distance from the door-panel to the parent, which is logically the distance to the hinge
			Rigidbody body = transform.parent.gameObject.GetComponent<Rigidbody>() as Rigidbody;
			Vector3 centerDelta = transform.position - body.position;

			return centerDelta * 2.0f;
		}
    }
	
	public Vector3 PivotPosition
	{
		get
		{
			Rigidbody body = transform.parent.gameObject.GetComponent<Rigidbody>() as Rigidbody;
			return body.position;
		}
	}
	
	private Interaction m_openInteraction 	= null;
	private Interaction m_closeInteraction 	= null;
	private float m_currentRotation = 0.0f;
	private float m_lerpProgress = 0.0f;
	private float m_lerpDirection = -1.0f;
	private float m_targetValue = 0.0f;
	private Quaternion m_initialRotation = Quaternion.identity;
	private DoorState m_doorState = DoorState.Closed;
}
