using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	public Vector3 offset = Vector3.zero;
	private GameObject m_player = null;

	public float LerpRate = 0.2f;
	
	// Use this for initialization
	void Start () 
	{
		m_player = GameObject.FindWithTag("Player");
		transform.position = m_player.transform.position + offset;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(m_player == null)
		{
			m_player = GameObject.FindWithTag("Player");
		}
		else
		{
			transform.position =  Vector3.Lerp(transform.position, new Vector3(m_player.transform.position.x,  m_player.transform.position.y, m_player.transform.position.z) + offset, LerpRate);
			transform.position = new Vector3(transform.position.x, m_player.transform.position.y + offset.y, transform.position.z);
		}
	}


}
