///////////////////////////////////////////////////////////
// 
// AI.cs
//
// What it does: The basic Monobehaviour controlling a given NPC's AI
//
// Notes: Contains a number of AIStates, each of which runs a set of behaviours.
//		  
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class AI : MonoBehaviour 
{

	void Start ()
	{
		m_currentState = m_states[m_startStateIndex];
		m_currentState.Start();
	}

	void Update ()
	{
		AIState nextState = m_currentState.Update();
		
		if(nextState != null)
		{
			m_currentState.End();
			m_currentState = nextState;
			m_currentState.Start();
		}
	}
	
	public void DeleteState(AIState state)
	{
		foreach(var currentState in m_states)
		{
			foreach(var behaviour in currentState.Behaviours)
			{
				if(behaviour.TransitionTarget == state)
				{
					behaviour.TransitionTarget = null;	
				}
			}
		}
		
		m_states.Remove(state);
	}
	
	public List<AIState> States
	{
		get { return m_states; }	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag	== "Player")
		{
			PlayerInPerceptionRange = true;	
		}
		
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag	== "Player")
		{
			PlayerInPerceptionRange = false;	
		}	
	}
	
	public int StartStateIndex
	{
		get { return m_startStateIndex; }
		set { m_startStateIndex = value; }
	}
	
	public int SelectedState { get; set; }
	
	public bool PlayerInPerceptionRange = false;
	
	[SerializeField]
	private List<AIState> m_states = new List<AIState>();
	
	[SerializeField]
	private int m_startStateIndex = 0;
	
	[SerializeField]
	private AIState m_currentState = null;
	
#if UNITY_EDITOR
	
	public AIBehaviour m_dragStart = null;
	
#endif
	
}
