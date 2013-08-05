using UnityEngine;
using System.Collections;

public class ThumbstickRotate : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 lastDirection = (Vector3.up * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		
		float angle = Mathf.Atan2(lastDirection.x, lastDirection.y);
		
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, -angle * Mathf.Rad2Deg);
		
		float magnitude = lastDirection.magnitude;
		transform.localScale = new Vector3(magnitude, magnitude, 1.0f);
			
			
	}
}
