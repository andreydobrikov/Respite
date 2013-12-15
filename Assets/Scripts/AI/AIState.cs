///////////////////////////////////////////////////////////
// 
// AIState.cs
//
// What it does: Encapsulates a single set of AIBehaviours.
//
// Notes: Patrol, pursue, idle, etc
// 
// To-do: All of it
//
///////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class AIState : ScriptableObject
{
	public AIState()
	{
		Running = false;
	}
	
	public void Start()
	{
		foreach(var behaviour in m_behaviours)
		{
			behaviour.Start();	
		}
		
		Running = true;
	}
	
	public AIState Update()
	{
		for(int i = 0; i < m_behaviours.Count; i++)
		{
			if(m_behaviours[i].Update())
			{
				if(m_behaviours[i].TransitionTarget == null)
				{
					Debug.LogWarning("Behaviour \"" + m_behaviours[i].Name + "\" requested a transition, but has no target set");	
				}
				else
				{
					return m_behaviours[i].TransitionTarget;		
				}
				
			}
		}
		
		return null;
	}	
	
	public void End()
	{
		foreach(var behaviour in m_behaviours)
		{
			behaviour.End();	
		}
		
		Running = false;
	}
	
	public string Name 
	{
		get { return m_name; }
		set { m_name = value; } 
	}
	
	public List<AIBehaviour> Behaviours 
	{
		get { return m_behaviours; }
		set { m_behaviours = value; }
	}
	
	public AI Parent
	{
		get { return m_parent; }	
		set { m_parent = value; }
	}
	
	public bool Running { get; set; }
	
	[SerializeField]
	private string m_name 					= String.Empty;
	
	[SerializeField]
	private List<AIBehaviour> m_behaviours 	= new List<AIBehaviour>();
	
	[SerializeField]
	private AI m_parent = null;
	
#if UNITY_EDITOR
	
	[SerializeField]
	public Rect m_editorPosition = new Rect(10, 10, 200, 20);

	[SerializeField]
	public Vector2 m_renderDimensions = Vector2.zero;

	[SerializeField]
	public Rect m_behaviourRenderRect;

	[SerializeField]
	public float m_windowWidth;
	
	[SerializeField]
	public bool m_showFoldout = false;

	[SerializeField]
	public bool m_showBehavioursFoldout = false;
	
	[SerializeField]
	public int m_behaviourToAdd = 0;
	
#endif
}
