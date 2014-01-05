using UnityEngine;
using System.Collections;

public class AIBehaviourTest : AIBehaviour 
{
	public AIBehaviourTest()
	{
		m_name = "Test";
		m_requiredTaskPaths.Add("test_task");
	}

	public override void Start()
	{

	}

	public override void Update()
	{
	}

	public override void Shutdown()
	{
		
	}

}
