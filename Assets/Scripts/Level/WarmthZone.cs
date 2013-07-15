using UnityEngine;
using System.Collections;

public class WarmthZone : MonoBehaviour 
{
	public float temperature = 1.0f;
	
	void OnTriggerEnter(Collider other)
	{
		Player player = other.GetComponent<Player>();	
		if(player != null)
		{
			m_player = player;	
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		Player player = other.GetComponent<Player>();	
		if(player != null)
		{
			m_player = null;	
		}	
	}
	
	void FixedUpdate()
	{
		if(m_player != null)
		{
			if(m_player.Warmth < temperature)
			{
				m_player.Warmth += (temperature * (Time.deltaTime / 100.0f));
				m_player.Warmth = Mathf.Min(m_player.Warmth, 1.0f);
			}
		}
	}
	
	private Player m_player = null;
}
