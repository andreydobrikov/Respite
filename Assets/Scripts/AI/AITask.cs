﻿/// <summary>
/// AI task.
/// 
/// An AI task is a set suspendable set of actions
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public sealed partial class AITask : ScriptableObject, IComparable 
{

#region Public methods

	public static AITask LoadTask(string taskName)
	{
		AITask newTask = new AITask();
		newTask.Deserialise(Application.dataPath + "/ai_tasks/" + taskName + ".json");

		return newTask;
	}

	public void Start()
	{
		Result = AITaskResult.Running;

		if(m_actions.Count > 0)
		{
			m_currentAction = m_actions[0];
			m_currentAction.Start();
		}
	}

	public void Suspend(AITaskSuspendPriority priority)
	{
		Result = AITaskResult.Suspended;
	}

	public void Update()
	{
		if(m_currentAction != null)
		{
			m_currentAction.Update();

			if(m_currentAction.Result == AIActionResult.Complete)
			{
				AIAction nextAction = m_currentAction.LinkedAction;

				if(nextAction != null)
				{
					//TODO: Reset current action here.
					m_currentAction.Stop();
					m_currentAction = nextAction;
					m_currentAction.Start();
				}
				else
				{
					Result = AITaskResult.Complete;
				}
			}
		}
	}

	public void Resume()
	{

	}

	public void Reset()
	{
		Result = AITaskResult.Idle;
	}

	public void AddAction(AIAction newAction)
	{
		newAction.Task = this;
		m_actions.Add(newAction);
	}

	public int CompareTo(object that)
	{
		AITask otherTask = (AITask)that;
		
		if(Priority == otherTask.Priority)	{ return 0; }
		if(Priority > otherTask.Priority)	{ return 1;	}
		
		return -1;
	}

	public AIAction GetActionFromSerialisationID(int id)
	{
		foreach(var action in m_actions)
		{
			if(action.SerialisationID == id)
			{
				return action;
			}
		}
		return null;
	}

#endregion

#if UNITY_EDITOR
	public bool Dirty = false;
#endif

#region Properties

	public AIBehaviour 		Behaviour { get; set; }
	public AIAction			EntryPoint { get; set; } 
	public AITaskPriority 	Priority { get; set; }
	public string 			Name { get; set; }
	public AITaskResult		Result { get; set; }
	public List<AIAction> 	Actions { get { return m_actions; } } 

#endregion

#region Private fields

	[SerializeField]
	private List<AIAction> m_actions 	= new List<AIAction>();

	[SerializeField]
	public string m_actiontest = "";
	private AIAction m_currentAction	= null;

#endregion

}
