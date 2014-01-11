using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract partial class AIAction : ScriptableObject
{
	public abstract void Init();
	public abstract void Start();
	public abstract void Update();
	public abstract void Stop();

	public void SetOutput(string outputName, AIAction target)
	{
		for (int index = 0; index < m_outputLinks.Count; index++)
		{
			if (m_outputLinks[index].linkName == outputName)
			{
				var temp = m_outputLinks[index];
				temp.linkAction = target;
				m_outputLinks[index] = temp;
				return;
			}
		}
		Debug.LogError("Action <b>" + m_name + "</b> has no output \"" + outputName + "\"");
	}

	public AIAction GetOutput(string outputName)
	{
		for (int index = 0; index < m_outputLinks.Count; index++)
		{
			if (m_outputLinks[index].linkName == outputName)
			{
				return m_outputLinks[index].linkAction;
			}
		}

		return null;
	}

	public AIAction GetOutput(int outputIndex)
	{
		return Outputs[outputIndex].linkAction;
	}

	public int SerialisationID			{ get; set; }
	public string Name 					{ get { return m_name; }	}
	public AITask Task 					{ get; set; }
	public AIActionResult Result 		{ get { return m_result; } }
	public List<AIActionLink> Outputs	{ get { return m_outputLinks; } }

	public AIAction LinkedAction	
	{ 
		get 
		{
			for (int index = 0; index < m_outputLinks.Count; index++)
			{
				if (m_outputLinks[index].linkName == m_targetLink)
				{
					return m_outputLinks[index].linkAction;
				}
			}
			return null;
		}
	}

	protected GameObject GetGameObject()
	{
		return Task.Behaviour.m_parentAI.gameObject;
	}

	protected AIActionData GetInputData(string dataName)
	{
		foreach(var data in m_inputData)
		{
			if(data.DataID == dataName)
			{
				return data;
			}
		}

		return null;
	}

	protected string m_name 			= string.Empty;
	protected string m_ID 				= string.Empty; // This is used as the ID during serialisation, so be wary of altering it!.
	protected string m_targetLink		= string.Empty;	// The link to follow upon completion; 
	protected AIActionResult m_result 	= AIActionResult.Idle;

	[SerializeField]
	public List<AIActionLink> m_outputLinks		= new List<AIActionLink>(); 
	[SerializeField]
	public List<AIActionData> m_inputData 		= new List<AIActionData>();
	[SerializeField]
	public List<AIActionData> m_outputData 		= new List<AIActionData>();

#if UNITY_EDITOR
	public Rect m_renderDimensions;

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

	[SerializeField]
	public bool m_showInputDataFoldout = false;

	[SerializeField]
	public bool m_showOutputDataFoldout = false;
	
#endif
}



