using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour 
{

	public GameObject Target = null;
	public Vector3 offset = Vector3.zero;

	// Update is called once per frame
	void LateUpdate () 
	{
		transform.position = Target.transform.position + offset;	
	}
}
