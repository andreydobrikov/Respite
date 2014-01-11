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

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AIManager : MonoBehaviour 
{
	public List<System.Type> AvailableBehaviours = new List<System.Type>();
	public List<string> AvailableBehaviourNames = new List<string>();
	
	void OnEnable()
	{
		RebuildActionList();
		s_instance = this;
		DoDeserialise();
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
			string fileName = System.IO.Path.GetFileNameWithoutExtension(task.Name);
			if(task.Extension == ".json" && !string.IsNullOrEmpty(fileName))
			{
				Debug.Log("Deserialising " + task.FullName);
				AITask newTask = ScriptableObject.CreateInstance(typeof(AITask)) as AITask;

				bool succeeded = newTask.Deserialise(System.IO.Path.ChangeExtension(task.FullName, null));

				if(succeeded)
				{
					m_tasks.Add(newTask);
					m_taskNames.Add(newTask.Name);
				}
				else
				{
					Debug.LogError("Failed to load " + task.FullName);
				}
			}
		}
		Debug.Log("Deserialisation complete");
	}

	public void RebuildActionList()
	{
		System.Type targetType = typeof(AIAction);
		List<System.Type> types = new List<System.Type>(targetType.Assembly.GetTypes().Where(x => x.IsSubclassOf(targetType)));

		m_actionNames.Clear();
		m_actionTypes.Clear();

		foreach (var actionType in types)
		{
			m_actionNames.Add(actionType.Name);
			m_actionTypes.Add(actionType);
		}
	}

#if UNITY_EDITOR
	public void ReloadTasks()
	{
		foreach(var task in m_tasks)
		{
			if(task.Dirty)
			{
				if(!EditorUtility.DisplayDialog("Unsaved Tasks", "Task \"" + task.Name + "\" has unsaved changes that will be lost. Continue?", "ok", "cancel"))
				{
					return;
				}
			}
		}

		m_taskNames.Clear();
		m_tasks.Clear();

		DoDeserialise();
	}

	public int selectedTaskIndex = 0;
	public int selectedActionIndex = 0;
	public List<string> m_taskNames = new List<string>();
	public List<string> m_actionNames = new List<string>();
	public List<System.Type> m_actionTypes = new List<System.Type>();
	public AIAction m_dragAction = null;
	public int m_dragActionOutput = -1;
	public bool m_debugEditor = true;
	public float m_zoom = 1.0f;
	public float m_buttonBarHeight = 0.0f;
#endif

	public List<AITask> m_tasks = new List<AITask>();
	
	public static AIManager s_instance = null;
}
 