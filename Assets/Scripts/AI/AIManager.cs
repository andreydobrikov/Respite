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

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AIManager : MonoBehaviour 
{
	public List<System.Type> AvailableBehaviours = new List<System.Type>();
	public List<string> AvailableBehaviourNames = new List<string>();
	
	void OnEnable()
	{
		AvailableBehaviours.Clear();	
		AvailableBehaviourNames.Clear();
		
		// TODO: I would love to find a way to automate this. 
		// You could do it by reflection I suppose.
		AvailableBehaviours.Add(typeof(AIBehaviourPatrol));
		AvailableBehaviours.Add(typeof(AIBehaviourActivateAlarms));
		AvailableBehaviours.Add(typeof(AIBehaviourWatchForPlayer));
		AvailableBehaviours.Add(typeof(AIBehaviourFollowPlayer));
		AvailableBehaviours.Add(typeof(AIBehaviourEndGame));
		
		foreach(var behaviour in AvailableBehaviours)
		{
			AvailableBehaviourNames.Add(behaviour.Name);
		}
		
		s_instance = this;
	}
	
	public static AIManager s_instance = null;
}
 