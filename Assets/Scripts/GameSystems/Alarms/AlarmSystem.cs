///////////////////////////////////////////////////////////
// 
// AlarmSystem.cs
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

public class AlarmSystem : MonoBehaviour 
{

	void Start ()
	{
		m_elements.AddRange(Resources.FindObjectsOfTypeAll(typeof(AlarmElement)) as AlarmElement[]);
		
		DeactivateAlarms();
	}

	public void ActivateAlarms()
	{
		foreach(var element in m_elements)
		{
			element.gameObject.SetActive(true);
		}
	}
	
	public void DeactivateAlarms()
	{
		foreach(var element in m_elements)
		{
			element.gameObject.SetActive(false);
		}	
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 40, 300, 30));
		
		GUILayout.BeginHorizontal((GUIStyle)("Box"));
		
		if(GUILayout.Button("Activate Alarms")) { ActivateAlarms(); }
		if(GUILayout.Button("Deactivate Alarms")) { DeactivateAlarms(); }
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}
		
	private List<AlarmElement> m_elements = new List<AlarmElement>();
}
