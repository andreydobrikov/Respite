using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class RoomComponent : MonoBehaviour 
{
	private MeshRenderer m_meshRenderer = null;
	private Room m_room = null;
	
	// Use this for initialization
	void Start () 
	{
		m_meshRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_meshRenderer.material.SetVector("_Ambient", m_room.ambientLight);
	}
	
	public void RegisterRoom(Room room)
	{
		m_room = room;	
	}
}
