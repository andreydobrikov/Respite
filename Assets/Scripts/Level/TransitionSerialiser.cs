///////////////////////////////////////////////////////////
// 
// TransitionController.cs
//
// What it does: Stores a List of GameObjects and serialises which are active.
//
// Notes: This is used to control which floors of a building are active when a save occurs
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SerialisableObject))]
public class TransitionSerialiser : MonoBehaviour, ISerialisable 
{
	public List<GameObject> TransitionObjects = new List<GameObject>();
	
	public void SaveSerialise(List<SavePair> pairs)
	{
		foreach(var current in TransitionObjects)
		{
			pairs.Add(new SavePair(current.name, current.activeInHierarchy ? "true" : "false"));		
		}
	}
	
	public void SaveDeserialise(List<SavePair> pairs)
	{
		Debug.Log("Deserialising building: " + name);
		
		// Horrible O(mn) loop, but it should only ever be dealing with about four items, so who gives a shit.
		foreach(var pair in pairs)
		{
			foreach(var currentObject in TransitionObjects)
			{
				if(pair.id == currentObject.name)
				{
					currentObject.SetActive(pair.value == "true" ? true : false);	
				}
			}
		}
		
		// Make sure the player object knows a transition has occured
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		
		player.BroadcastMessage("OnRegionTransition", SendMessageOptions.DontRequireReceiver);
	}
}
