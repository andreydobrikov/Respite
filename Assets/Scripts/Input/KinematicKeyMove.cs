using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class KinematicKeyMove : MonoBehaviour {
	
	
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
		m_controller.velocity = new Vector3(0.0f, 0.0f, 0.0f);
		if(Input.GetKey(KeyCode.UpArrow) )
		{
			
			m_controller.AddForce(transform.rotation * (Vector3.up * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			m_controller.AddForce(transform.rotation * (Vector3.up * -MoveSpeed));
			//m_controller.AddForce(new Vector3(0.0f, 0.0f, -MoveSpeed));
			//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (0.01f * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(0.0f, 0.0f, TurnSpeed);
			//m_controller.AddForce(new Vector3(-MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x - (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(0.0f, 0.0f, -TurnSpeed);
			//m_controller.AddForce(new Vector3(MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x + (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
	}
	
	void OnCollisionEnter(Collision other)
	{
		Debug.Log("COllided");	
	}
}
