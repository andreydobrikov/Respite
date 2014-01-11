using UnityEngine;
using System.Collections;

public class AIActionTest : AIAction 
{
	public AIActionTest()
	{
		m_name = "DEBUG";

		AIActionData data0 	= new AIActionData();
		AIActionData data1 	= new AIActionData();
		data0.ActionID 		= "action_input";
		data0.BlackBoardID 	= "test_blackboard_id";
		data0.DataType 		= typeof(float);

		data1.ActionID 		= "action_input1";
		data1.BlackBoardID 	= "test_blackboard_id1";
		data1.DataType 		= typeof(float);

		m_inputData.Add(data0);
		m_inputData.Add(data1);
		m_outputData.Add(data1);

		m_outputs.Add("thing");
		m_outputs.Add("thing0");
		m_outputs.Add("thing1");
		m_outputs.Add("thing2");
		m_outputs.Add("thing3");
		m_outputs.Add("thing4");
		m_outputs.Add("thing5");
		m_outputs.Add("thing6");
		m_outputs.Add("thing7");
		m_outputs.Add("thing8");

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
