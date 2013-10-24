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
	public override void Start()
	{
		m_name = "End Game";
		m_supportTransitions = false;
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
