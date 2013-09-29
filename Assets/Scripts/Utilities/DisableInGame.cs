///////////////////////////////////////////////////////////
// 
// DisableInGame.cs
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

public class DisableInGame : MonoBehaviour 
{
	void OnEnable()
	{
		gameObject.SetActive(false);	
	}
	
}
