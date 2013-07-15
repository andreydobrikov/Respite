using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowWrapper : MonoBehaviour 
{
	public GameObject LevelObject;
		
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
	
	void OnLevelWasLoaded(int levelID)
	{
		
	}
	
	public List<GameObject> AgentStartupItems
	{
		get { return m_agentStartupObjects; }
		set { m_agentStartupObjects = value; }
	}
	
	public List<GameObject> AdminStartupItems
	{
		get { return m_adminStartupObjects; }
		set { m_adminStartupObjects = value; }
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10.0f, Screen.height - 70.0f, 300.0f, 60.0f));
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Save"))
		{
			Serialiser.Instance.Serialise();	
		}
		
		if(GUILayout.Button("Load"))
		{
			Serialiser.Instance.Deserialise();	
		}
		
		GUILayout.EndHorizontal();
		
		
		Serialiser.Instance.OutputDebugInfo = GUILayout.Toggle(Serialiser.Instance.OutputDebugInfo, "Debug Info");
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	[SerializeField]
	private List<GameObject> m_agentStartupObjects = new List<GameObject>();
	
	[SerializeField]
	private List<GameObject> m_adminStartupObjects = new List<GameObject>();
}
