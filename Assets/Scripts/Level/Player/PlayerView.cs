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
	public float InspectionAngleMax 	= 60.0f;
	public float InspectionFocusAngle 	= 10.0f;
	public float LerpTime				= 0.3f;

	void Update ()
	{
		Vector3 inputDirection 	= (Vector3.up * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		float inputMagnitude 	= inputDirection.magnitude;
		
		inputDirection.Normalize();
		
		Vector3 origin 			= transform.position;
		Vector3 playerDirection = transform.localRotation * Vector3.up;
		float diff 				= Mathf.Acos(Vector3.Dot(playerDirection, inputDirection)) * Mathf.Rad2Deg;
		
		
		//m_lastDirection = inputDirection;
		
		Vector3 cross 	= Vector3.Cross(inputDirection, playerDirection);
		float diffSign 	= -Vector3.Dot(Vector3.forward, cross);
		
		if(diffSign != 0.0f)
		{
			diffSign = diffSign / Mathf.Abs(diffSign);
			
			diff *= diffSign;
			
			diff = Mathf.Clamp(diff, -InspectionAngleMax, InspectionAngleMax);
			Debug.Log(diff);
		}
		
		
		m_target = playerDirection;
		
		if(inputMagnitude > 0.2f)
		{
			m_target = Quaternion.Euler(0.0f, 0.0f, diff) * playerDirection;
		}
		
		float targetAngle 	= -Mathf.Atan2(m_target.x, m_target.y)  * Mathf.Rad2Deg;
		float lastAngle 	= -Mathf.Atan2(m_lastDirection.x, m_lastDirection.y)  * Mathf.Rad2Deg;
		
		Quaternion lastRotation 	= Quaternion.Euler(0.0f, 0.0f, lastAngle);
		Quaternion targetRotation 	= Quaternion.Euler(0.0f, 0.0f,targetAngle);
		
		float diffAngle = Quaternion.Angle(targetRotation, lastRotation);
		
		if(diffAngle > 0.0f)
		{
			m_lastDirection = Quaternion.Slerp(lastRotation, targetRotation, LerpTime) * Vector3.up;
		}
		else
		{
			m_lastDirection = m_target;	
		}
		
		m_lastDirectionAngle = -Mathf.Atan2(m_lastDirection.x, m_lastDirection.y) * Mathf.Rad2Deg;
		
		Debug.DrawLine(transform.position, transform.position + m_target, Color.yellow);
		Debug.DrawLine(transform.position, transform.position + m_lastDirection, Color.green);
		//Debug.DrawLine(transform.position, transform.position + m_lastDirection, Color.red);
		
		
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
	private Vector3 m_target = Vector3.up;
	
}
