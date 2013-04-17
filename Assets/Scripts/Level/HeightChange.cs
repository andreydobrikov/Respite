using UnityEngine;
using System.Collections;

public class HeightChange : MonoBehaviour 
{

	public float NewHeight = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
			other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, NewHeight);
	}
}
