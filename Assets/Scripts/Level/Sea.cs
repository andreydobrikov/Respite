using UnityEngine;
using System.Collections;

public class Sea : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		m_player = GameObject.FindGameObjectWithTag("Player");
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// TODO: This turns off the ocean when underground. Perhaps something a little more robust would be sensible
		if(m_player != null)
		{
			if(m_player.transform.position.y < -0.1f)
			{
				renderer.enabled = false;
			}
			else
			{
				renderer.enabled = true;
			}
		}
	}

	GameObject m_player = null;
}
