///////////////////////////////////////////////////////////
// 
// SettingsEditor.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SettingsEditor :  EditorWindow 
{
    [MenuItem ("Respite/Settings")]
    static void ShowWindow () 
	{
        EditorWindow.GetWindow(typeof(SettingsEditor));
    }

    void OnGUI () 
	{
		
		s_style = new GUIStyle((GUIStyle)("Button"));
		
		//Object test = AssetDatabase.LoadAssetAtPath("Assets/icons/trash.png", typeof(Texture2D));
		//s_style.normal.background = test as Texture2D;
		//s_style.normal.textColor = Color.red;
		
		m_toDelete.Clear();
		
        Settings settings = Settings.Instance;
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		foreach(var setting in Settings.Instance.Values)
		{
			GUILayout.BeginHorizontal();
			
			setting.key 	= EditorGUILayout.TextField(setting.key);
			setting.value 	= EditorGUILayout.TextField(setting.value);
			
			if(GUILayout.Button("delete", GUILayout.Width(80)))
			{
				m_toDelete.Add(setting);
			}
			
			GUILayout.EndHorizontal();
		}
		
		foreach(var toDelete in m_toDelete)
		{
			Settings.Instance.DeleteSetting(toDelete);	
		}
		
		GUILayout.EndVertical();
		
		if(GUILayout.Button("Add", GUILayout.Width(50)))
		{
			Settings.Instance.UpdateSetting("new_setting", "")	;
		}
		
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Save", GUILayout.Width(60)))
		{
			Settings.Instance.SaveSettings();	
		}
		
		if(GUILayout.Button("Load", GUILayout.Width(60)))
		{
			Settings.Instance.LoadSettings();	
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
    }
	
	static GUIStyle s_style = null;
	
	private List<Settings.Setting> m_toDelete = new List<Settings.Setting>();
}
