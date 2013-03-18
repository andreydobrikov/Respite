using UnityEngine;
using System.Collections;

public class DoorSensor : MonoBehaviour 
{
	public string TriggerTag = "Player";
	
	private Door m_door;
	private int m_detectedEntities = 0;
	
	// Use this for initialization
	void Start () 
	{
		m_door = transform.parent.gameObject.GetComponent<Door>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag == TriggerTag)
		{
			m_detectedEntities--;	
			
			if(m_detectedEntities == 0)
			{
				Debug.Log("Closing door");
				m_door.SetState( Door.DoorState.Closed );	
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == TriggerTag)
		{
			m_detectedEntities++;
			m_door.SetState( Door.DoorState.Open );	
			Debug.Log("Opening door");
		}
	}
}
