using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ActorController))]
public class KeyMove : MonoBehaviour {
	
	
	public float MoveSpeed = 0.1f;
	public float JumpPower = 0.1f;
	
	ActorController m_controller = null;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<ActorController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(Input.GetKey(KeyCode.UpArrow) )
		{
			m_controller.AddVelocity(new Vector3(0.0f, MoveSpeed, 0.0f));
		}
		
		if(Input.GetKey(KeyCode.DownArrow))
		{
			m_controller.AddVelocity(new Vector3(0.0f, -MoveSpeed, 0.0f));
			//transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (0.01f * MoveSpeed));
		}
		
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			m_controller.AddVelocity(new Vector3(-MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x - (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			m_controller.AddVelocity(new Vector3(MoveSpeed, 0.0f, 0.0f));
			//transform.position = new Vector3(transform.position.x + (0.01f * MoveSpeed), transform.position.y, transform.position.z);
		}
		
	}
}
