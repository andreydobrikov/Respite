///////////////////////////////////////////////////////////
// 
// AIManager.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.IO;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AIManager : MonoBehaviour 
{
	public List<System.Type> AvailableBehaviours = new List<System.Type>();
	public List<string> AvailableBehaviourNames = new List<string>();
	
	void OnEnable()
	{
		AvailableBehaviours.Clear();	
		AvailableBehaviourNames.Clear();

		/*
		// TODO: I would love to find a way to automate this. 
		// You could do it by reflection I suppose.
		AvailableBehaviours.Add(typeof(AIBehaviourPatrol));
		AvailableBehaviours.Add(typeof(AIBehaviourActivateAlarms));
		AvailableBehaviours.Add(typeof(AIBehaviourWatchForPlayer));
		AvailableBehaviours.Add(typeof(AIBehaviourFollowPlayer));
		AvailableBehaviours.Add(typeof(AIBehaviourEndGame));
		*/
		foreach(var behaviour in AvailableBehaviours)
		{
			AvailableBehaviourNames.Add(behaviour.Name);
		}
		
		s_instance = this;

		m_tasks.Clear();

		AITask task = new AITask();
		task.Name = "test_task";

		AIAction action0 = new AIActionTest();
		AIAction action1 = new AIActionTest();
		AIAction action2 = new AIActionTest();
		task.AddAction(action0);
		task.AddAction(action1);
		task.AddAction(action2);

		m_tasks.Add(task);

		AITask patrolTask = new AITask();
		action0 = new AIActionNavigate();
		action0.SetOutput("nav_complete", action0);

		patrolTask.Name = "patrol_task";

		patrolTask.AddAction(action0);

		m_tasks.Add(patrolTask);
	}

	public void DoSerialise()
	{
		Debug.Log("Serialising tasks...");
		foreach(var task in m_tasks)
		{
			Debug.Log("\t<b>" + task.Name + "</b>");
			task.Serialise(Application.dataPath + "/ai_tasks");
		}
		Debug.Log("Complete");
	}

	public void DoDeserialise()
	{
		Debug.Log("Deserialising tasks...");
		m_tasks.Clear();

		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/ai_tasks");

		foreach(var task in info.GetFiles())
		{
			Debug.Log("Checking " + task);
			if(task.Extension == ".json")
			{
				Debug.Log("Deserialising " + task.FullName);
				AITask newTask = new AITask();

				newTask.Deserialise(System.IO.Path.ChangeExtension(task.FullName, null));

				m_tasks.Add(newTask);
			}
		}
		Debug.Log("Deserialisation complete");
	}


	private List<AITask> m_tasks = new List<AITask>();
	
	public static AIManager s_instance = null;
}
 