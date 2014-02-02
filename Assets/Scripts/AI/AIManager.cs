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
#if AI_LOGGING
		Debug.Log("<color=green>(AI)Serialising tasks...</color>");
#endif
		foreach(var task in m_tasks)
		{
#if AI_LOGGING
			Debug.Log("\t<b>" + task.Name + "</b>");
#endif
			task.Serialise(Application.dataPath + "/resources/ai_tasks");
		}
#if AI_LOGGING
		Debug.Log("Complete");
#endif
	}

	public void DoDeserialise()
	{
#if AI_LOGGING
		Debug.Log("<color=green>(AI)Deserialising Tasks</color>");
#endif

		m_tasks.Clear();
		var tasks = Resources.LoadAll("ai_tasks");


		foreach(var task in tasks)  
		{
			TextAsset asset = task as TextAsset;

			if(asset == null)
			{
				continue;
			}
#if AI_LOGGING
			Debug.Log (asset.name); 
#endif

			AITask newTask = ScriptableObject.CreateInstance(typeof(AITask)) as AITask;

			bool succeeded = newTask.Deserialise(asset.text);

			if(succeeded)
			{
				m_tasks.Add(newTask); 
				 
#if UNITY_EDITOR
				m_taskNames.Add(newTask.Name);
#endif
			}
			else
			{
				Debug.LogError("Failed to load " + asset.name);
			}

		}
	}

	public void RebuildActionList()
	{
		System.Type targetType = typeof(AIAction);
		List<System.Type> types = new List<System.Type>(targetType.Assembly.GetTypes().Where(x => x.IsSubclassOf(targetType)));

#if UNITY_EDITOR
		m_actionNames.Clear();
		m_actionTypes.Clear();

		foreach (var actionType in types)
		{
			m_actionNames.Add(actionType.Name);
			m_actionTypes.Add(actionType);
		}
#endif
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
 