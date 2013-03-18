using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	
	public int MenuWidth = 200;
	public int MenuHeight = 180;
	
	void OnGUI()
	{
		int left = (Screen.width / 2) - MenuWidth / 2;
		int top = (Screen.height / 2) - MenuHeight / 2;
		GUI.Box (new Rect(left, top, MenuWidth, MenuHeight), "Game Mode");
		
		if(GUI.Button(new Rect(left + 10, top + 30, MenuWidth - 20, 60), "Agent")) 
		{
			GameFlow.Instance.View = WorldView.Agent;
			Application.LoadLevel("AgentView");
		}	
		
		if(GUI.Button(new Rect(left + 10, top + 100, MenuWidth - 20, 60), "Admin")) 
		{
			GameFlow.Instance.View = WorldView.Admin;
			Application.LoadLevel("AgentView");
		}
	}
}
