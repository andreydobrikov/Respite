using UnityEngine;
using System.Collections;

public class UndergroundWeatherMask : MonoBehaviour {

	// Use this for initialization
	void Start () {
		m_player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_player != null)
		{
			if(m_player.transform.position.y < -1.0f)
			{
				renderer.enabled = true;
			}
			else
			{
				renderer.enabled = false;
			}
		}
	
	}
	private GameObject m_player = null;
}
