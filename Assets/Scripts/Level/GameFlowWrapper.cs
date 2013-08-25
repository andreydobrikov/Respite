using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFlowWrapper : MonoBehaviour 
{
	public GameObject LevelObject;
	
#if UNITY_EDITOR
	public bool ShowFoldout = false;
#endif
	
	void Start()
	{
		GameFlow instance = GameFlow.Instance;
		
		instance.SaveFadeDuration = m_saveFadeDuration;
		m_started = true;
		
		string[] names = Input.GetJoystickNames();
		
		foreach(var name in names)
		{
			Debug.Log(name);	
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameFlow.Instance.Update();
	}
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10.0f, Screen.height - 120.0f, 300.0f, 100.0f));
		GUILayout.BeginVertical();
		
		GUILayout.Label(System.Enum.GetName(typeof(GameFlow.ControlContext), GameFlow.Instance.CurrentControlContext));
		
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
	
	public float SaveFadeDuration
	{
		get { return GameFlow.Instance.SaveFadeDuration; }
		set 
		{
			GameFlow.Instance.SaveFadeDuration = value;
			m_saveFadeDuration = value;
		}
	}
	
	void OnApplicationFocus(bool focus)
	{
		if(GameFlow.Instance.CurrentControlContext != GameFlow.ControlContext.Menu && !focus && m_started)
		{
		//	GameFlow.Instance.RequestMenu();	
		}
	}
	
	private bool m_started 				= false;
	private float m_saveFadeDuration 	= 3.0f;
}
