///////////////////////////////////////////////////////////
// 
// Settings.cs
//
// What it does: Stores settings, you prick.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

[Serializable]
public class Settings 
{
	public static Settings Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new Settings();
				s_instance.LoadSettings();
			}	
			
			return s_instance;
		}
	}
	
	private Settings()
	{
			
	}
	
	public void UpdateSetting(string key, string value)
	{
		Setting setting = m_settings.Find(x => x.key == key);
		
		if(setting != null)
		{
			setting.value = value;	
		}
		else
		{
			setting = new Setting();
			
			setting.key = key;
			setting.value = value;
			
			m_settings.Add(setting);
		}
	}
	
#if UNITY_EDITOR
	public void DeleteSetting(Setting setting)
	{
		bool okay = EditorUtility.DisplayDialog("Delete Setting", "This will delete the setting \"" + setting.key + "\"", "okay", "cancel");			
		
		if(okay)
		{
			m_settings.Remove(setting);	
		}
	}
#endif
	
	public string GetSetting(string key)
	{
		string outValue = null;
		
		Setting setting = m_settings.Find(x => x.key == key);
		
		if(setting == null)
		{
			Debug.LogWarning(key + " not found in settings");
		}
		else
		{
			outValue = setting.value;
		}
		
		return outValue;
	}
	
	public void LoadSettings()
	{
		m_settings.Clear();
		
		using(TextReader tr = File.OpenText(@"C:\json.txt"))
		{
			using(JsonReader reader = new JsonTextReader(tr))
			{
				string currentName = string.Empty;
				
				while(reader.Read())
				{
					if(reader.TokenType == JsonToken.PropertyName)
					{
						currentName = reader.Value as string;		
						Debug.Log("Grabbed " + currentName);
					}
					else if(reader.TokenType == JsonToken.String)
					{
						string val = reader.Value as string;
						Setting newSetting = new Setting();
						
						newSetting.key = currentName;
						newSetting.value = val;
						
						m_settings.Add(newSetting);
					}
				}
			}
		}
	}
	
	public void SaveSettings()
	{
		using (StreamWriter sw = new StreamWriter(@"c:\json.txt"))
		{
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.WriteStartObject();

				foreach(var setting in m_settings)
				{
					writer.WritePropertyName(setting.key);
					writer.WriteValue(setting.value);	
				}
				
				writer.WriteEndObject();
			}
		}
	}
	
	public List<Setting> Values
	{
		get { return m_settings; }	
	}
	 
	private static Settings s_instance = null;
	
	private List<Setting> m_settings = new List<Setting>();
	
	[Serializable]
	public class Setting
	{
		public string key;
		public string value;	
	}
}
