/// <summary>
/// Game object helper.
/// 
/// Static helper functions for common GameObject tasks.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GameObjectHelper 
{
	public static GameObject FindChild(GameObject gameObject, string childName, bool clearSubChildren)
	{
		GameObject child = null;
		
		for(int i = 0; i < gameObject.transform.childCount && child == null; ++i)
		{
			Transform currentChild = gameObject.transform.GetChild(i);
			
			if(currentChild.gameObject.name == childName)
			{
				child = currentChild.gameObject;
			}
		}
		
		if(child == null)
		{
			child = new GameObject(childName);
			child.transform.parent = gameObject.transform;
		}
		else if(clearSubChildren)
		{
			while(child.transform.childCount > 0)
			{
				GameObject.DestroyImmediate(child.transform.GetChild(0).gameObject);
			}
		}
		
		return child;
	}
	
	// Searches up :{
	public static GameObject SearchForComponent(GameObject searchFocus, System.Type targetType)
	{
		GameObject current = searchFocus.transform.parent.gameObject;
		
		while(current != null)
		{
			// Search breadth then go up.
			int childCount = current.transform.GetChildCount();
			Component searchComponent = null;
			
			for(int i = 0; i < childCount; ++i)
			{
				GameObject child = current.transform.GetChild(i).gameObject;
				
				if(child == searchFocus)
				{
					continue;	
				}
				
				searchComponent = child.GetComponent(targetType);
				if(searchComponent != null)
				{
					return searchComponent.gameObject;		
				}
			}
			
			// Search the node itself. This is needed for the root node. TODO: Only do it then, duh.
			searchComponent = current.gameObject.GetComponent(targetType);
			if(searchComponent != null)
			{
				return searchComponent.gameObject;		
			}
			
			current = current.transform.parent.gameObject;	
		}
		
		return null;
	}
	
	public static void LogQuaternionEuler(Quaternion quaternion)
	{
		Debug.Log("Quaternion: " + quaternion.eulerAngles.x + ", " + quaternion.eulerAngles.y + ", " + quaternion.eulerAngles.z);
	}
}
