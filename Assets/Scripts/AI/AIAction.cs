using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract partial class AIAction : ScriptableObject
{
	public abstract void Start();
	public abstract void Update();
	public abstract void Stop();

	public void SetOutput(string outputName, AIAction target)
	{
		for (int index = 0; index < m_outputs.Count; index++)
		{
			if (m_outputs[index] == outputName)
			{
				m_links[index] = target;
				return;
			}
		}
		Debug.LogError("Action <b>" + m_name + "</b> has no output \"" + outputName + "\"");
	}

	public AIAction GetOutput(string outputName)
	{
		for (int index = 0; index < m_outputs.Count; index++)
		{
			if (m_outputs[index] == outputName)
			{
				return m_links[index];
			}
		}

		return null;
	}

	public AIAction GetOutput(int outputIndex)
	{
		return GetOutput(Outputs[outputIndex]);
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
			for (int index = 0; index < m_outputs.Count; index++)
			{
				if (m_outputs[index] == m_targetLink)
				{
					return m_links[index];
				}
			}
			return null;
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

	[SerializeField]
	protected List<string> m_outputs				= new List<string>(); 
	[SerializeField]
	protected List<AIActionData> m_inputData 		= new List<AIActionData>();
	[SerializeField]
	protected List<AIActionData> m_outputData 		= new List<AIActionData>();
	[SerializeField]
	protected List<AIAction> m_links 	= new List<AIAction>();

#if UNITY_EDITOR
	public Rect m_renderDimensions;

	[SerializeField]
	public List<Rect> m_outputRects = new List<Rect>();
	[SerializeField]
	public List<Rect> m_inputDataRects = new List<Rect>();
	[SerializeField]
	public List<Rect> m_outputDataRects = new List<Rect>();

	[SerializeField]
	public Rect m_lastBounds;
	[SerializeField]
	public Rect m_editorPosition	= new Rect(100, 100, 100, 100);
	[SerializeField]
	public float m_windowWidth		= 100; 
	
#endif
}

[Serializable]
public struct AIActionData
{
	[SerializeField]
	public string BlackBoardID;

	[SerializeField]
	public string ActionID;

	[SerializeField]
	public System.Type DataType; 
}

public enum AIActionResult
{
	Idle,
	Running,
	Complete
}