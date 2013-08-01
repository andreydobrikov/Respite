using UnityEngine;
using System.Collections;

public class InspectionGUI : MonoBehaviour 
{
	public float InspectionAngleMax = 60.0f;
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 targetDirection = (Vector3.up * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		//targetDirection.y = -targetDirection.y;
		
		Vector3 origin = transform.position;
		
		float angle = -(Mathf.Rad2Deg * Mathf.Atan2(targetDirection.x, targetDirection.y));
		if(angle < 0.0f)
		{
			angle += 360.0f;	
		}
		
		float diff = Mathf.Abs(transform.localRotation.eulerAngles.z - angle);
		
		diff = (diff + 180.0f);
		if(diff >= 360.0f) diff = diff - 360.0f;
		diff -= 180.0f;
		diff = Mathf.Abs(diff);
		
		if(diff < InspectionAngleMax)
		{
		
			Debug.DrawLine(origin, origin + targetDirection, Color.magenta);
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10.0f, 300.0f, 100.0f, 30.0f), "Inspection test");
	}
}
