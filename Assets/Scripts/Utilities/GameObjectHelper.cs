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
}
