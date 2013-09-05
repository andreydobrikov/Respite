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
	public static float BoxColliderHeight = 3;
	
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
	
	#endif
}
