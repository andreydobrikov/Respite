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
using System.Linq;
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
		
		//Object test = AssetDatabase.LoadAssetAtPath("Assets/icons/trash.png", typeof(Texture2D));
		//s_style.normal.background = test as Texture2D;
		//s_style.normal.textColor = Color.red;
		
		m_toDelete.Clear(); 
		
		GUILayout.BeginVertical();

		var types = GetSettingTypes();

		string[] typeStrings = new string[types.Count];

		int index = 0;
		foreach (var currentType in types)
		{
			typeStrings[index] = currentType.Name;
			index++;
		}

		EditorGUILayout.BeginVertical((GUIStyle)("Box"));

		Settings.showAddSettingFoldout = EditorGUILayout.Foldout(Settings.showAddSettingFoldout, "Add Setting");

		if (Settings.showAddSettingFoldout)
		{
			Settings.selectedSettingTypeIndex = EditorGUILayout.Popup(Settings.selectedSettingTypeIndex, typeStrings);

			EditorGUILayout.BeginHorizontal();
			Settings.newSettingName = EditorGUILayout.TextField(Settings.newSettingName);


			if (string.IsNullOrEmpty(Settings.newSettingName) || Settings.Instance.Values.ContainsKey(Settings.newSettingName))
			{
				GUI.enabled = false;
			}

			if (GUILayout.Button("Add Setting", GUILayout.MaxWidth(100)))
			{
				Settings.Instance.AddSetting(types[Settings.selectedSettingTypeIndex], Settings.newSettingName);
			}
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}

		
		EditorGUILayout.EndVertical();

		if (Settings.Instance.Values.Count > 0)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));

			foreach (var setting in Settings.Instance.Values)
			{
				GUILayout.BeginHorizontal();

				//	setting.key 	= EditorGUILayout.TextField(setting.key);
				//	setting.value 	= EditorGUILayout.TextField(setting.value);
				setting.Value.OnInspectorGUI();
				if (GUILayout.Button("delete", GUILayout.Width(60)))
				{
					m_toDelete.Add(setting.Value);
				}

				GUILayout.EndHorizontal();
			}

			foreach (var toDelete in m_toDelete)
			{
				Settings.Instance.DeleteSetting(toDelete);
			}

			GUILayout.EndVertical();
		}
		
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Reload", GUILayout.Width(60)))
		{
			Settings.Instance.LoadSettings();	
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
    }
	
	private List<Setting> m_toDelete = new List<Setting>();

	private List<System.Type> GetSettingTypes()
	{
		System.Type targetType = typeof(Setting);
		List<System.Type> types = new List<System.Type>(targetType.Assembly.GetTypes().Where(x => x.IsSubclassOf(targetType)));

		return types;
	}
}
