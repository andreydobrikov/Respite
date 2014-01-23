///////////////////////////////////////////////////////////
// 
// DimensionBodger.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class DimensionBodger : MonoBehaviour 
{
	public static float BoxColliderHeight = 2;
	
	#if UNITY_EDITOR
	
	[MenuItem ("Respite/Bodge/Set Box-Collider Heights")]
	public static void SetAllBoxColliderHeight()
	{
		
		BoxCollider[] colliders = Resources.FindObjectsOfTypeAll(typeof(BoxCollider)) as BoxCollider[];
		foreach(var collider in colliders)
		{
			collider.size = new Vector3(collider.size.x, BoxColliderHeight, collider.size.z);
		}
		
	}


	[MenuItem ("Respite/Bodge/Force Light offsets")]
	#endif
	public static void ForceLightOffsets()
	{
		List<GameObject> lights = new List<GameObject>();
		
		GameObject[] objects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
		
		int lightLayer = LayerMask.NameToLayer("Lights");
		
		foreach(var current in objects)
		{
			if(current.layer == lightLayer)
			{
				lights.Add(current);	
			}
		}
		
		float minY = float.MaxValue;
		float maxY = float.MinValue;
		
		foreach(var light in lights)
		{
			if(light.transform.localPosition.y < minY) { minY = light.transform.localPosition.y; }
			if(light.transform.localPosition.y > maxY) { maxY = light.transform.localPosition.y; }
		}
		
		float delta = 0.2f / (lights.Count - 1);
		
		for(int i = 0; i < lights.Count; ++i)
		{
			Vector3 localPosition = lights[i].transform.localPosition;
			localPosition.y = 2.0f + (delta * i);
			
			lights[i].transform.localPosition = localPosition;
			
			//Debug.Log("Bodged: " + lights[i].name);
		}
		
	}
	
	//#endif
}
