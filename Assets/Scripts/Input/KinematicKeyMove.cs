using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class KinematicKeyMove : MonoBehaviour 
{
	public float MoveSpeed = 200f;
	public float TurnSpeed = 2.0f;
	
	Rigidbody m_controller = null;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		m_controller.velocity = Vector3.zero;
			m_controller.AddForce( (Vector3.up * (Input.GetAxis("Vertical") * MoveSpeed)));
			m_controller.AddForce( (Vector3.right * (Input.GetAxis("Horizontal") * MoveSpeed)));
	
	}
}
