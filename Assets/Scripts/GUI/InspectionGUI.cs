using UnityEngine;
using System.Collections;

public class InspectionGUI : MonoBehaviour 
{
	void Start()
	{
		m_gameFlow = GameFlow.Instance;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_gameFlow != null)
		{
			if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.Inspection)
			{
				return; 	
			}
			
			if(Input.GetButtonDown("back"))
			{
				m_gameFlow.EndInspection();	
			}
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10.0f, 300.0f, 100.0f, 30.0f), "Inspection test");
	}
		
	private GameFlow m_gameFlow = null;
}
