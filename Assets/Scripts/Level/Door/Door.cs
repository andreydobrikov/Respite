using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
	public enum DoorState
	{
		Open,
		Closed
	}
	
	
	public DoorState State 
	{
		get; set;	
	}
	
	Door()
	{
		State = DoorState.Closed;	
	}
	
	void Update()
	{
		//TODO: This is all guff for the admin-view
		
		
	}
	
	void Start()
	{
		for(int i = 0; i < transform.GetChildCount(); ++i)
		{
			if(transform.GetChild(i).gameObject.GetComponent<MeshRenderer>() != null)
			{
				m_geoObject = (GameObject)transform.GetChild(i).gameObject;
			}
		}
	}
	
	public void SetState(DoorState state)
	{
		if(state != State)
		{
			MeshRenderer renderer = m_geoObject.GetComponent<MeshRenderer>();
			if(state == DoorState.Open)
			{
				
				{
					Vector3 newTransform = m_geoObject.transform.position;
					newTransform += m_geoObject.transform.TransformDirection(Vector3.up) * renderer.bounds.size.x;
					m_geoObject.transform.position = newTransform;
				}
			}
			else
			{
			//	MeshRenderer renderer = transform.GetComponentInChildren<MeshRenderer>();
				{
					Vector3 newTransform = m_geoObject.transform.position;
					newTransform -= m_geoObject.transform.TransformDirection(Vector3.up)  * renderer.bounds.size.x;
					m_geoObject.transform.position = newTransform;
				}
			}
			
			State = state;
		}
	}
	
	private GameObject m_geoObject = null;
}
