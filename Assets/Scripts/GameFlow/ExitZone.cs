///////////////////////////////////////////////////////////
// 
// ExitZone.cs
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

public class ExitZone : MonoBehaviour 
{

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameFlow.Instance.RequestMenu();
		}
	}
}
