using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		m_gameFlow = GameFlow.Instance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.Menu)
		{
			if(Input.GetKeyDown("escape"))
			{
				GameFlow.Instance.RequestMenu();	
			}
			
			return;	
		}
		
		if(Input.GetKeyDown("escape"))
		{
			GameFlow.Instance.EndMenu();	
		}
	
	}
	
	void OnGUI()
	{
		if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.Menu)
		{
			return;	
		}
		
		GUI.Label(new Rect(Screen.width / 2.0f - 50.0f, Screen.height / 2.0f - 50.0f, 100, 100), "MENU, INNIT");
	}
			
	GameFlow m_gameFlow = null;	
}
