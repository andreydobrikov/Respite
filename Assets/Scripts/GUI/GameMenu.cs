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
		if(m_gameFlow == null)
		{
			m_gameFlow = GameFlow.Instance;	
		}
		
		if(m_gameFlow.CurrentControlContext == GameFlow.ControlContext.GameOver)
		{
			if(Input.GetButtonDown("escape"))
			{
				m_gameFlow.ResetLevel();
				
			}
			return;
		}
		
		if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.Menu)
		{
			if(Input.GetButtonDown("escape"))
			{
				GameFlow.Instance.RequestMenu();	
			}
			
			return;	
		}
		
		if(Input.GetButtonDown("escape"))
		{
			GameFlow.Instance.EndMenu();	
		}
	
	}
	
	void OnGUI()
	{
		GUI.depth = -11;
		
		// TODO: Switch, duh
		if(m_gameFlow.CurrentControlContext == GameFlow.ControlContext.Menu)
		{
			GUI.Label(new Rect(Screen.width / 2.0f - 50.0f, Screen.height / 2.0f - 50.0f, 100, 100), "MENU, INNIT");	
		}
		
		if(m_gameFlow.CurrentControlContext == GameFlow.ControlContext.GameOver)
		{
			GUI.Label(new Rect(Screen.width / 2.0f - 50.0f, Screen.height / 2.0f - 50.0f, 100, 100), "Game Over");
		}
		
		if(m_gameFlow.CurrentControlContext == GameFlow.ControlContext.Loading)
		{
			m_loadingInt++;
			m_loadingInt = m_loadingInt % 3;
			
			string progress = "Loading";
			for(int i = 0; i < m_loadingInt; i++)
			{
				progress += ".";	
			}
			
			GUI.Label(new Rect(Screen.width / 2.0f - 50.0f, Screen.height / 2.0f - 50.0f, 100, 100), progress);
		}
		
	}
	
	private static int m_loadingInt = 0;
	GameFlow m_gameFlow = null;	
}
