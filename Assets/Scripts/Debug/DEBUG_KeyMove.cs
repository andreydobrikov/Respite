using UnityEngine;
using System.Collections;

public class DEBUG_KeyMove : MonoBehaviour {
	
	public float MoveSpeed = 1.0f;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Input.GetKey(KeyCode.UpArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + (0.01f * MoveSpeed), transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - (0.01f * MoveSpeed), transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.position = new Vector3(transform.position.x - (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.position = new Vector3(transform.position.x + (0.01f  * MoveSpeed), transform.position.y, transform.position.z);
		}
	}
}
