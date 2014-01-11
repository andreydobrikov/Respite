using UnityEngine;
using System.Collections;

public class AIActionIdle : AIAction 
{
	public AIActionIdle()
	{
		m_name = "Idle";



	}

	public override void Init()
	{
		AIActionData idleTimeData = ScriptableObject.CreateInstance(typeof(AIActionData)) as AIActionData;
		AIActionLink actionLink = ScriptableObject.CreateInstance(typeof(AIActionLink)) as AIActionLink;
		actionLink.linkName = "idle_complete";
		
		
		idleTimeData.DataID = "input_idle_time";
		idleTimeData.DataType = typeof(float).AssemblyQualifiedName;
		
		m_inputData.Add(idleTimeData);
		m_outputLinks.Add(actionLink);
	}

	public override void Start()
	{
		var idleTimeData = GetInputData("input_idle_time");

		if (!Task.Behaviour.m_parentAI.Blackboard.GetEntry<float>(idleTimeData.BlackboardSourceID.GetHashCode(), ref m_idleTime))
		{
			Debug.LogWarning("Behaviour expects blackboard data \"" + idleTimeData.BlackboardSourceID + "\". Data not found.");
		}

		m_progress = 0.0f;

		m_result = AIActionResult.Running;
	}

	public override void Update()
	{
		m_progress += GameTime.DeltaTime;

		if(m_progress > m_idleTime)
		{
			m_targetLink = "idle_complete";
			m_result = AIActionResult.Complete;
		}
	}

	public override void Stop()
	{
		m_result = AIActionResult.Idle;
	}

	private float m_idleTime = 0.0f;
	private float m_progress = 0.0f;

	private AIActionData m_idleTimeData;
}
