/// <summary>
/// AI action link.
/// 
/// Connects two AIActions together.
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;


[Serializable]
public class AIActionLink : ScriptableObject
{
	[SerializeField]
	public string linkName;
	
	[SerializeField]
	public AIAction linkAction;
	
	#if UNITY_EDITOR
	[SerializeField]
	public Rect outputRect;
	#endif
	
}