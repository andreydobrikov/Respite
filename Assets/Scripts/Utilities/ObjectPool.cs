///////////////////////////////////////////////////////////
// 
// ObjectPool.cs
//
// What it does: Pre-builds a load of objects and activates/deactivates them as requested.
//
// Notes: Saves that ruddy instantiation cost. 
//		  Toggles the Active state, so don't use for objects with static colliders. Why they would be getting
//	      dynamically created anyway, I don't know.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
 
public class ObjectPool : MonoBehaviour 
{
	public PoolableObject PoolObject;
	public int MaxObjects = 50;
	
	void Start ()
	{
		// Make sure a specific GameObject has been specified
		if(PoolObject == null)
		{
			Debug.LogError("Object Pool Object not set");	
			return;
		}
		
		// Fill the inactive list with the desired number of objects
		for(int i = 0; i < MaxObjects; i++)
		{
			GameObject newObject = GameObject.Instantiate(PoolObject.gameObject) as GameObject;
			newObject.SetActive(false);
			newObject.transform.parent = transform;
			
			// Set the owner of the poolable object, in case it wants to deactivate itself.
			PoolableObject newPoolObject = newObject.GetComponent<PoolableObject>() as PoolableObject;
			newPoolObject.SetPool(this);
			
			m_inactivatePool.AddLast(newObject);	
		}
	}
	
	public GameObject ActivateObject()
	{
		GameObject activeObject = m_inactivatePool.Last.Value as GameObject;
		m_inactivatePool.RemoveLast();
		
		if(activeObject != null)
		{
			activeObject.SetActive(true);
			return activeObject;	
		}
		
		Debug.LogWarning("ObjectPool out of objects!");
		return null;
	}
	
	public void DeactivateObject(GameObject targetObject)
	{
		targetObject.SetActive(false);
		m_inactivatePool.AddLast(targetObject);
	}
	
	[SerializeField]
	private LinkedList<GameObject> m_inactivatePool = new LinkedList<GameObject>();
}
