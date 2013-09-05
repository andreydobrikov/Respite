///////////////////////////////////////////////////////////
// 
// PlayerView.cs
//
// What it does: Sorts where the player is peeking
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class PlayerView : MonoBehaviour 
{
	public float InspectionAngleMax 	= 60.0f; // The player's view angle
	public float InspectionFocusAngle 	= 10.0f; // The angle in which a player can inspect an item
	public float LerpTime				= 0.3f;

	void Update ()
	{
		// Grab the controller input it, store the magnitude, normalize the vector.
		Vector3 inputDirection 	= (Vector3.forward * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		float inputMagnitude 	= inputDirection.magnitude;
		
		inputDirection.Normalize();
		
		// Get a vector representing the player's current rotation
		Vector3 playerDirection = transform.localRotation * Vector3.forward;
		
		// Find the angle between the player and the input directin
		float diff 				= Mathf.Acos(Vector3.Dot(playerDirection, inputDirection)) * Mathf.Rad2Deg;
		
		// To find out the sign of that angle the first step is to get a perpendicular.
		// Taking the dot of the perpendicular and up will give 1 if they are the same, -1 if opposite. Zero if no input vector is zero.
		Vector3 cross 	= Vector3.Cross(inputDirection, playerDirection);
		
		// TODO: The normalize here is quite costly and the same effect can be achieved by the val/abs(val) code below,
		// but it makes for nice safe output when testing (diffSign must be 1, -1, 0 with the normalized version)
		float diffSign 	= -Vector3.Dot(Vector3.up, cross.normalized);
		
		// diffSign can be zero if no input direction is given.
		if(diffSign != 0.0f)
		{
			// This is redundant as the dot product will always be 1 or -1
			diffSign = diffSign / Mathf.Abs(diffSign);
			
			diff *= diffSign;
			
			diff = Mathf.Clamp(diff, -InspectionAngleMax, InspectionAngleMax); 
		}
		
		m_target = playerDirection;
		
		if(inputMagnitude > 0.2f)
		{
			m_target = Quaternion.Euler(0.0f, diff, 0.0f) * playerDirection;
		}
		
		float targetAngle 	= Mathf.Atan2(m_target.x, m_target.z)  * Mathf.Rad2Deg;
		float lastAngle 	= Mathf.Atan2(m_lastDirection.x, m_lastDirection.z)  * Mathf.Rad2Deg;
		
		Quaternion lastRotation 	= Quaternion.Euler(0.0f, lastAngle, 0.0f);
		Quaternion targetRotation 	= Quaternion.Euler(0.0f, targetAngle, 0.0f);
		
		float diffAngle = Quaternion.Angle(targetRotation, lastRotation);
		
		if(diffAngle > 0.0f)
		{
			m_lastDirection = Quaternion.Slerp(lastRotation, targetRotation, LerpTime) * Vector3.forward;
		}
		else
		{
			m_lastDirection = m_target;	
		}
		
		m_lastDirectionAngle = Mathf.Atan2(m_lastDirection.x, m_lastDirection.z) * Mathf.Rad2Deg;
		
		Debug.DrawLine(transform.position, transform.position + m_target, Color.yellow);
		Debug.DrawLine(transform.position, transform.position + m_lastDirection, Color.green);
	}
	
	public Vector3 Direction
	{
		get { return m_lastDirection; }	
	}
	
	public float DirectionAngle
	{
		get { return m_lastDirectionAngle; }	
	}
	
	private Vector3 m_lastDirection = Vector3.zero;
	private float m_lastDirectionAngle = 0.0f;
	private Vector3 m_target = Vector3.forward;
	
}
