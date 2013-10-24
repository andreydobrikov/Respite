///////////////////////////////////////////////////////////
// 
// AIBehaviour.cs
//
// What it does: Runs a specific AI action, such as movement
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public abstract class AIBehaviour : ScriptableObject
{
	public AIBehaviour()
	{
		m_lastBounds = new Rect(0.0f, 0.0f, 100.0f, 100.0f);	
	}
	
	public abstract void Start();
	public abstract bool Update();
	public abstract void End();
	
	protected GameObject GetObject()
	{
		return m_parentState.Parent.gameObject;
	}
	
#if UNITY_EDITOR
	
	public virtual void OnSceneGUI() {}
	public virtual void OnInspectorGUI() {}
	
#endif 
	
	public AIState Parent
	{
		get { return m_parentState; }
		set { m_parentState = value; }
	}
	
	public string Name
	{
		get { return m_name; }
	}
	
	public AIState TransitionTarget
	{
		get { return m_targetTransition; }
		set { m_targetTransition = value; }
	}
	
	public bool SupportsTransitions
	{
		get { return m_supportTransitions;	}
	}
	
	[SerializeField]
	protected string m_name = String.Empty;
	
	[SerializeField]
	protected bool m_supportTransitions = false;
	
	[SerializeField]
	public AIState m_parentState = null;
	
	[SerializeField]
	public AIState m_targetTransition = null;
	
	
	[SerializeField]
	public Rect m_lastBounds;
	
#if UNITY_EDITOR
	
	[SerializeField]
	public bool m_showFoldout = false;
	
#endif
	
}
