///////////////////////////////////////////////////////////
// 
// ProgressBar.cs
//
// What it does: Scales two things according to a float. CRAZY
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class ProgressBar : MonoBehaviour 
{
	public GameObject background;
	public GameObject fill;
	
	public float progress = 0.0f;

	void Start ()
	{
		initialFillScale 		= background.transform.localScale;
	}

	void Update ()
	{
		transform.localRotation = Quaternion.identity;
		progress = Mathf.Clamp(progress, 0.0f, 1.0f);
		
		Vector3 newScale = Vector3.Lerp(Vector3.zero, initialFillScale, progress);
		fill.transform.localScale = newScale;
	}
	
	private Vector3 initialFillScale 		= Vector3.one;
}
