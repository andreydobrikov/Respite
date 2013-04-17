using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour 
{
	public Color test;
	
	public Vector4 ambientLight
	{
		get { return m_ambientLight; }	
	}
	
	// Use this for initialization
	void Start () 
	{
		m_ambientLight = test;
		
		
		RoomComponent[] components = transform.GetComponentsInChildren<RoomComponent>();
		foreach(RoomComponent component in components)
		{
			component.RegisterRoom(this);
			m_components.Add(component);	
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private List<RoomComponent> m_components = new List<RoomComponent>();
	private Vector4 m_ambientLight = new Vector4();
}
