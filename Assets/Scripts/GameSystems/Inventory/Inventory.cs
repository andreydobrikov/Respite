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
		int index = 0;
		
		foreach(var item in Inventory.Contents)
		{          
			SerialisableObject itemSave = item.gameObject.GetComponent<SerialisableObject>() as SerialisableObject;
			if(itemSave != null)
			{
				pairs.Add(new SavePair("inventory_item_" + index, itemSave.GUID.ToString()));
				index++;
			}
			else
			{
				Debug.LogWarning("Cannot save inventory-item \"" + item.name + "\" as it does not have a SerialisableObject	attached");
			}
		}
	}
	
	public static void SaveDeserialise(List<SavePair> pairs)
	{
		Debug.Log("Deserialising inventory...");
		
		// Flush the current dictionary
		Inventory.Contents.Clear();
		
		InventoryObject[] inventoryObjects = Resources.FindObjectsOfTypeAll(typeof(InventoryObject)) as InventoryObject[];
		
		// Create a dictionary of inventory objects with their guids as a key.
		// This should make a shift to O(m+n) instead of O(mn)
		Dictionary<string, InventoryObject> objectDictionary = new Dictionary<string, InventoryObject>();
		foreach(var current in inventoryObjects)
		{
			SerialisableObject serialisableObject = current.GetComponent<SerialisableObject>();
			
			if(serialisableObject != null)
			{
				objectDictionary.Add(serialisableObject.GUID, current);
			}
			else
			{
				Debug.LogWarning("Cannot deserialise inventory-item \"" + current.name + "\" as it does not have a SerialisableObject	attached");
			}
		}
		
		// Run through each guid and add the corresponding InventoryObject.
		// The objects themselves are deserialised in their own scripts.
		foreach(var pair in pairs)
		{
			string guid = pair.value;
			
			InventoryObject target = null;
			
			if(objectDictionary.TryGetValue(guid, out target))
			{
				Debug.Log("Deserialised: " + target.name);
				Inventory.AddToInventory(target);
			}
			else
			{
				Debug.LogWarning("Failed to deserialise inventory-object with guid: " + guid);	
			}
		}
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
