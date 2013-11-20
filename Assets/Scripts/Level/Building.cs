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
	public int floorHeight			= 0;
	public bool BuildFog			= false;
	public bool lightsActive		= false;
	
	public Building()
	{
		Rooms.Add(new Room());	
	}
	
	public void Start()
	{
		m_lights = GameObjectHelper.FindAllChildrenWithLayer(this.gameObject, LayerMask.NameToLayer("Lights"));
		m_lights.AddRange(GameObjectHelper.FindAllChildrenWithLayer(this.gameObject, LayerMask.NameToLayer("Shadow")));
		//Debug.Log("Building \"" + BuildingName + "\" Found " + m_lights.Count + " lights");
		
	}
	
	public void DisableLights()
	{
		foreach(var light in m_lights)
		{
			if(light.renderer != null)
			{
				light.renderer.enabled = false;	
			}
		}
		
		lightsActive = false;
	}
	
	public void EnableLights()
	{
		foreach(var light in m_lights)
		{
			if(light.renderer != null)
			{
				light.renderer.enabled = true;	
			}
		}
		
		lightsActive = true;
	}
		
	private List<GameObject> m_lights = null;
	
#if UNITY_EDITOR
		
	public static string s_walls_id 			= "walls";
	public static string s_rooms_id 			= "rooms";
	public static string s_floor_id 			= "floor";
	public static string s_ambient_id 			= "ambient";
	public static string s_fog_id 				= "fog";
	public static string s_weather_mask_id 		= "weather_mask";
	public static string s_weather_mask_mat_id 	= "DepthMask";
	
#endif
	
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
