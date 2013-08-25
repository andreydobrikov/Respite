///////////////////////////////////////////////////////////
// 
// InventoryInterface.cs
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

public class Inventory
{
	public static void AddToInventory(InventoryObject newObject)
	{
		if(Instance.m_objects.Contains(newObject))
		{
			Debug.LogWarning("Object " + newObject + " already in inventory");	
		}
		else
		{
			Instance.m_objects.Add(newObject);
			newObject.transform.position = OOWPosition;
		}
	}
	
	public static void SaveSerialise(List<SavePair> pairs)
	{
		
	}
	
	public static void SaveDeserialise(List<SavePair> pairs)
	{
		
	}
	
	public static List<InventoryObject> Contents
	{
		get { return Instance.m_objects; }	
	}
	
	private static Inventory Instance
	{
		get	
		{
			if(s_inventory == null)
			{
				s_inventory = new Inventory();	
			}
			
			return s_inventory;
		}
	}
	
	private Inventory()
	{
		
	}
	
	private static Inventory s_inventory = null;
	
	private List<InventoryObject> m_objects = new List<InventoryObject>();
	private static Vector3 OOWPosition = new Vector3(-100.0f, -100.0f, 0.0f);
}
