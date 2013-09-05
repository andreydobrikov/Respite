///////////////////////////////////////////////////////////
// 
// Guard.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Guard : MonoBehaviour 
{

	void Start ()
	{
		m_agent = GetComponent<NavMeshAgent>();	
		m_agent.SetDestination(transform.position + new Vector3(0.0f, 0.0f, 20.0f));
		m_player = GameObject.FindGameObjectWithTag("Player");
		
	//	m_agent.autoTraverseOffMeshLink = false;
	}

	void Update ()
	{
		if(m_agent.isOnOffMeshLink)
		{
						Debug.Log("Off mesh");	
			m_agent.transform.position = m_agent.currentOffMeshLinkData.endPos;
			
			m_agent.CompleteOffMeshLink();
		}
		else
		{
			//Debug.Log("Position: "+ m_player.transform.position.x + ", " + m_player.transform.position.y);
			m_agent.SetDestination(m_player.transform.position + new Vector3(0.0f, 0.0f, -1.0f));	
		}
		
		
	}
	
	private GameObject m_player = null;
	private NavMeshAgent m_agent = null;
}
