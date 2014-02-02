///////////////////////////////////////////////////////////
// 
// AIBehaviour.cs
//
// What it does: An AI behaviour monitors an individual aspect of the AI, such as 
//				 following a task schedule, socialising, responding to environmental stimuli, etc.
//				 These behaviours can issue task requests to direct the entity itself.
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

	public void LoadTasks()
	{
#if AI_LOGGING
		Debug.Log("Loading <b>" + m_requiredTaskPaths.Count + "</b> tasks for behaviour <b>" + m_name + "</b>");
#endif
		foreach(var task in m_requiredTaskPaths)
		{
			AITask newTask = AITask.LoadTask(task);

			if(newTask != null)
			{
				newTask.Behaviour = this;
				m_tasks.Add(task, newTask);
			}
			else
			{
				Debug.LogError("Failed to load task: " + task);
			}
		}
	}

	public virtual void RegisterBlackboardEntries() {}

	public abstract void Start();
	public abstract void Update();
	public abstract void Shutdown();
	
#if UNITY_EDITOR
	
	public virtual void OnSceneGUI() {}
	public virtual void OnInspectorGUI() {}
	
#endif 
	
	public string Name
	{
		get { return m_name; }
	}
	
	public bool Enabled 
	{ 
		get { return m_enabled; }
		set { m_enabled = value; }
	}

	[SerializeField]
	protected string m_name = String.Empty;
	
	[SerializeField]
	public Rect m_lastBounds;

	[SerializeField]
	public bool m_enabled;

	[SerializeField]
	public AI m_parentAI;

	protected List<string> m_requiredTaskPaths = new List<string>();
	protected Dictionary<string, AITask> m_tasks = new Dictionary<string, AITask>();
	
#if UNITY_EDITOR
	
	[SerializeField]
	public bool m_showFoldout = false;
	
#endif
	
}
