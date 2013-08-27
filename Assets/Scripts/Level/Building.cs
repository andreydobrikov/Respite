///////////////////////////////////////////////////////////
// 
// Building.cs
//
// What it does: Really just a stub to store information so that editor scripts can rebuild buildings easily.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour 
{
	public string BuildingName 		= string.Empty;
	public List<Room> Rooms			= new List<Room>();
	public bool ShowRoomsFoldout	= false;
	
	public Building()
	{
		Rooms.Add(new Room());	
	}

}

[Serializable]
public class Room
{
	public Room()
	{
		Name = "New Room";	 
	}
	
	[SerializeField]
	public string Name;
	
	[SerializeField]
	public Color TODMaxColor = Color.white;
	
	[SerializeField]
	public Color TODMinColor = Color.white;
	
	[SerializeField]
	public bool ShowFoldout = false;
}
