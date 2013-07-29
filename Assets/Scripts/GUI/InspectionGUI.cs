using UnityEngine;
using System.Collections;

public class InspectionGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 targetDirection = (Vector3.up * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		//targetDirection.y = -targetDirection.y;
		
		Vector3 origin = transform.position + new Vector3(0.0f, 0.0f, -1.0f);
		
		Debug.DrawLine(origin, origin + targetDirection, Color.magenta);
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10.0f, 300.0f, 100.0f, 30.0f), "Inspection test");
	}
}
