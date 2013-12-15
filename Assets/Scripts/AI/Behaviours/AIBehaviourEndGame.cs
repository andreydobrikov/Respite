///////////////////////////////////////////////////////////
// 
// AIBehaviourEndGame.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class AIBehaviourEndGame : AIBehaviour 
{
	public AIBehaviourEndGame()
	{
		m_name = "End Game";
		m_supportTransitions = false;
	}

	public override void Start()
	{

		GameFlow.Instance.GameOver();	
	}
	
	public override bool Update()
	{
		return true;
	}
	
	public override void End()
	{
		
	}
}
