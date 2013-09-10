///////////////////////////////////////////////////////////
// 
// AIBehaviourActivateAlarms.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class AIBehaviourActivateAlarms : AIBehaviour 
{
	public AIBehaviourActivateAlarms()
	{
		m_name = "Activate Alarms";	
		m_supportTransitions = true;
	}
	
	public override void Start() 
	{ 
		m_alarms = FindObjectOfType(typeof(AlarmSystem)) as AlarmSystem;
		
		if(Deactivate)
		{
			m_alarms.DeactivateAlarms();	
		}
		else
		{
			m_alarms.ActivateAlarms();	
		}
		
		m_countdown = 10.0f;
	}
	
	public override bool Update() 
	{ 
		m_countdown -= Time.deltaTime;
		
		if(m_countdown < 0.0f)
		{
			return true;	
		}
		
		return false;
	}
	
	public override void End() 
	{
	}
	
	
#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		Deactivate = GUILayout.Toggle(Deactivate, "Deactivate");
	}
#endif
	
	[SerializeField]
	public bool Deactivate = false;
	
	private AlarmSystem m_alarms = null;
	private float m_countdown = 0.0f;
}
