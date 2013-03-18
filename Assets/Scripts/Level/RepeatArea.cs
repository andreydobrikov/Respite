using UnityEngine;
using System.Collections;

public class RepeatArea : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerExit(Collider other)
	{
		Debug.Log("Exiting trigger");	
	}
}
