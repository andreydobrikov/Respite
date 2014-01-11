using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class AIActionData : ScriptableObject
{
	[SerializeField]
	public string BlackboardSourceID;
	
	[SerializeField]
	public string DataID;
	
	[SerializeField]
	public string DataType; 
}