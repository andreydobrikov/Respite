using UnityEngine;
using System.Collections;

public class AIActionTest : AIAction 
{
	public AIActionTest()
	{
		m_name = "DEBUG";

	}

	public override void Init()
	{
		
		AIActionData data0 	= ScriptableObject.CreateInstance(typeof(AIActionData)) as AIActionData;
		AIActionData data1 	= ScriptableObject.CreateInstance(typeof(AIActionData)) as AIActionData;
		data0.DataID 		= "action_input";
		data0.BlackboardSourceID 	= "test_blackboard_id";
		data0.DataType 		= typeof(float).AssemblyQualifiedName;
		
		data1.DataID 		= "action_input1";
		data1.BlackboardSourceID 	= "test_blackboard_id1";
		data1.DataType 		= typeof(float).AssemblyQualifiedName;
		
		m_inputData.Add(data0);
		m_inputData.Add(data1);
		m_outputData.Add(data1);
		
		AIActionLink link = ScriptableObject.CreateInstance(typeof(AIActionLink)) as AIActionLink;
		link.linkName = "complete";
		
		m_outputLinks.Add(link);
		/*
		m_outputLinks.Add("thing");
		m_outputLinks.Add("thing0");
		m_outputLinks.Add("thing1");
		m_outputLinks.Add("thing2");
		m_outputLinks.Add("thing3"); 
		m_outputLinks.Add("thing4");
		m_outputLinks.Add("thing5");
		m_outputLinks.Add("thing6");
		m_outputLinks.Add("thing7");
		m_outputLinks.Add("thing8");
		*/
	}

	public override void Start()
	{
		m_agent = Task.Behaviour.m_parentAI.GetComponent<NavMeshAgent>();
		m_agent.destination = GameObject.FindGameObjectWithTag("Player").transform.position - new Vector3(0.0f, 0.0f, 2.0f);
	}

	public override void Update()
	{
		Debug.Log("Processing Test Action...");
	}

	public override void Stop()
	{

	}

	private NavMeshAgent m_agent = null;
}
