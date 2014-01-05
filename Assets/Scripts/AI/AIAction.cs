using UnityEngine;
using System.Collections.Generic;

public abstract partial class AIAction
{
	public abstract void Start();
	public abstract void Update();
	public abstract void Stop();

	public void SetOutput(string outputName, AIAction target)
	{
		if(m_outputs.Contains(outputName))
		{
			m_links.Remove(outputName);
			m_links.Add(outputName, target);
		}
		else
		{
			Debug.LogError("Action <b>" + m_name + "</b> has no output \"" + outputName + "\"");
		}
	}

	public int SerialisationID		{ get; set; }
	public string Name 				{ get { return m_name; }	}
	public AITask Task 				{ get; set; }
	public AIActionResult Result 	{ get { return m_result; } }
	public List<string> Outputs		{ get { return m_outputs; } }
	public AIAction LinkedAction	
	{ 
		get 
		{ 
			AIAction linkedAction = null;
			m_links.TryGetValue(m_targetLink, out linkedAction);
			return linkedAction;
		}
	}

	protected GameObject GetGameObject()
	{
		return Task.Behaviour.m_parentAI.gameObject;
	}

	protected string m_name 			= string.Empty;
	protected string m_ID 				= string.Empty; // This is used as the ID during serialisation, so be wary of altering it!.
	protected string m_targetLink		= string.Empty;	// The link to follow upon completion; 
	protected AIActionResult m_result 	= AIActionResult.Idle;

	protected List<string> m_outputs				= new List<string>();
	protected List<AIActionData> m_inputData 		= new List<AIActionData>();
	protected List<AIActionData> m_outputData 		= new List<AIActionData>();
	protected Dictionary<string, AIAction> m_links 	= new Dictionary<string, AIAction>();
}

public struct AIActionData
{
	public string BlackBoardID;
	public string ActionID;
	public System.Type DataType;
}

public enum AIActionResult
{
	Idle,
	Running,
	Complete
}