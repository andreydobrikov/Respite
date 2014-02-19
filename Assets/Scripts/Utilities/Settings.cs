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
#if UNITY_EDITOR
	public static bool showAddSettingFoldout = false;

	public static int selectedSettingTypeIndex = 0;
	public static string newSettingName = string.Empty;
	public static int labelWidth = 200;
#endif

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
	
	private Settings() { }
	
#if UNITY_EDITOR
	public Setting AddSetting(Type type, string key)
	{
		Setting newSetting = Activator.CreateInstance(type) as Setting;
		newSetting.SetKey(key);
		m_settings.Add(key, newSetting);

		SaveSettings();

		return newSetting;
	}

	public void DeleteSetting(Setting setting)
	{
		bool okay = EditorUtility.DisplayDialog("Delete Setting", "This will delete the setting \"" + setting.Key + "\"", "okay", "cancel");			
		
		if(okay)
		{
			m_settings.Remove(setting.Key);	
		}

		SaveSettings();
	}
#endif
	
	// Get an individual setting object
	public T GetSetting<T>(string key) where T : Setting
	{
		Setting setting = default(T) as Setting;
		
		if(!m_settings.TryGetValue(key, out setting))
		{
			Debug.LogWarning(key + " not found in settings");
			return null; 
		}
		else
		{
			T returnValue = (T)setting;
			if (returnValue == null)
			{
				Debug.LogWarning("Cannot convert setting \"" + key + "\" to type \"" + typeof(T).Name + "\"");
			}
			return returnValue;
		}
	}
	
	public void LoadSettings()
	{
		m_settings.Clear();

		Setting currentSetting = null;
		
		using(TextReader tr = File.OpenText(Application.dataPath + "\\resources\\settings.json"))
		{
			using(JsonReader reader = new JsonTextReader(tr))
			{
				while(reader.Read())
				{
					if(reader.TokenType == JsonToken.PropertyName)
					{
						if (reader.Value.ToString() == "type") 
						{
							// If there's an existing setting, add it to the list
							if (currentSetting != null) { m_settings.Add(currentSetting.Key, currentSetting); }

							// Read to the value 
							reader.Read();

							// Create the setting from the type
							string typeString = reader.Value.ToString();
							Type settingType = Type.GetType(typeString);
							currentSetting = Activator.CreateInstance(settingType) as Setting; 

							currentSetting.Deserialise(reader);

							
						}
					}
				}
				if (currentSetting != null) { m_settings.Add(currentSetting.Key, currentSetting); }
			}
		}
 	}
	
	public void SaveSettings()
	{
		using (StreamWriter sw = new StreamWriter(Application.dataPath + "\\resources\\settings.json"))
		{
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;

				writer.WriteStartObject();

				writer.WritePropertyName("settings_list");
				writer.WriteStartArray();
				foreach(var setting in m_settings)
				{
					writer.WriteStartObject();
					
					writer.WritePropertyName("type");
					writer.WriteValue(setting.Value.GetType().AssemblyQualifiedName);

					setting.Value.Serialise(writer);

					writer.WriteEndObject();
				}
				writer.WriteEndArray();

				writer.WriteEndObject();
			}
		}
	}

	public Dictionary<string, Setting> Values
	{
		get { return m_settings; }	
	}
	 
	private static Settings s_instance = null;
	
	[SerializeField]
	private Dictionary<string, Setting> m_settings = new Dictionary<string, Setting>();
}

[Serializable]
public abstract class Setting
{
	public delegate void SettingChanged();

#if UNITY_EDITOR
	public abstract void OnInspectorGUI();
#endif

	public abstract void Serialise(JsonWriter writer);
	public abstract void Deserialise(JsonReader reader);

	public void SetKey(string key)
	{
		if (!m_keySet)
		{
			m_key = key;
			m_keySet = true;
		}
		else
		{
			Debug.LogWarning("Key already set: " + key);
		}
	}

	public void AddChangedCallback(SettingChanged callback)
	{
		m_changedCallbacks.Add(callback);
	}

	public void RemoveChangedCallback(SettingChanged callback)
	{
		m_changedCallbacks.Remove(callback);
	}

	protected void FireSettingChanged()
	{
		foreach (var callback in m_changedCallbacks)
		{
			callback();
		}
	}

	public string Key { get { return m_key; } }

	[SerializeField]
	protected string m_key;

	[SerializeField]
	private bool m_keySet = false;

	protected List<SettingChanged> m_changedCallbacks = new List<SettingChanged>();
}

//////////////////////////////////////////////////////////////////////////////////
/// StringSetting
//////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class StringSetting : Setting
{
	public StringSetting() { }

	public StringSetting(string key, string value)
	{
		m_key	= key;
		m_value = value;
	}

#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		string oldValue = m_value;
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label(Key, GUILayout.MaxWidth(Settings.labelWidth));
		m_value = EditorGUILayout.TextField(m_value);

		EditorGUILayout.EndHorizontal();

		if (m_value != oldValue) 
		{
			FireSettingChanged();
			Settings.Instance.SaveSettings();  
		}
	}
#endif

	public override void Serialise(JsonWriter writer)
	{
		writer.WritePropertyName("key");
		writer.WriteValue(m_key);

		writer.WritePropertyName("value");
		writer.WriteValue(m_value);
	}

	public override void Deserialise(JsonReader reader)
	{
		reader.Read();
		reader.Read();

		m_key = reader.Value.ToString();

		reader.Read();
		reader.Read();

		// Read the key
		m_value = reader.Value.ToString();
	}

	public string Value 
	{
		get { return m_value; }
		set { m_value = value; }
	}
	 
	[SerializeField]
	protected string m_value = string.Empty;
}

//////////////////////////////////////////////////////////////////////////////////
/// FloatSetting
//////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class FloatSetting : Setting
{
	public FloatSetting() { }

	public FloatSetting(string key, float value)
	{
		m_key = key;
		m_value = value;
	}

#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		float oldValue = m_value;

		EditorGUILayout.BeginHorizontal();

		GUILayout.Label(Key, GUILayout.MaxWidth(Settings.labelWidth));
		m_value = EditorGUILayout.FloatField(m_value);

		EditorGUILayout.EndHorizontal();

		if (m_value != oldValue) 
		{
			FireSettingChanged();
			Settings.Instance.SaveSettings(); 
		}
	}
#endif

	public override void Serialise(JsonWriter writer)
	{
		writer.WritePropertyName("key");
		writer.WriteValue(m_key);

		writer.WritePropertyName("value");
		writer.WriteValue(BitConverter.DoubleToInt64Bits(m_value).ToString());
	}

	public override void Deserialise(JsonReader reader)
	{
		reader.Read();
		reader.Read();

		m_key = reader.Value.ToString();

		reader.Read();
		reader.Read();

		long val = 0;
		
		long.TryParse(reader.Value.ToString(), out val);
		
		// Read the key
		m_value = (float)BitConverter.Int64BitsToDouble(val);
	}

	public float Value
	{
		get { return m_value; }
		set { m_value = value; }
	}

	[SerializeField]
	protected float m_value = 0.0f;
}

//////////////////////////////////////////////////////////////////////////////////
/// FloatSettingRange
//////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class FloatSettingRange : FloatSetting
{
	public FloatSettingRange() { }

	public FloatSettingRange(string key, float value, float min, float max)
	{
		m_key = key;
		m_value = value;
	}

#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		float oldValue	= m_value;
		float oldMin	= m_min;
		float oldMax	= m_max;

		EditorGUILayout.BeginHorizontal();

		GUILayout.Label(Key, GUILayout.MaxWidth(Settings.labelWidth));
		m_min = EditorGUILayout.FloatField(m_min, GUILayout.MaxWidth(20));
		m_value = GUILayout.HorizontalSlider(m_value, m_min, m_max);
		m_max = EditorGUILayout.FloatField(m_max, GUILayout.MaxWidth(20));

		GUI.enabled = false;
		EditorGUILayout.FloatField(m_value, GUILayout.Width(30));
		GUI.enabled = true;

		EditorGUILayout.EndHorizontal();

		if (m_value != oldValue || m_min != oldMin || m_max != oldMax) 
		{
			FireSettingChanged();
			Settings.Instance.SaveSettings(); 
		}
	}
#endif

	public override void Serialise(JsonWriter writer)
	{
		writer.WritePropertyName("key");
		writer.WriteValue(m_key);

		writer.WritePropertyName("value");
		writer.WriteValue(BitConverter.DoubleToInt64Bits(m_value).ToString());

		writer.WritePropertyName("min");
		writer.WriteValue(BitConverter.DoubleToInt64Bits(m_min).ToString());

		writer.WritePropertyName("max");
		writer.WriteValue(BitConverter.DoubleToInt64Bits(m_max).ToString());
	}

	public override void Deserialise(JsonReader reader)
	{
		reader.Read();
		reader.Read();

		m_key = reader.Value.ToString();

		reader.Read();
		reader.Read();

		long val = 0;
		long.TryParse(reader.Value.ToString(), out val);
		m_value = (float)BitConverter.Int64BitsToDouble(val);

		reader.Read();
		reader.Read();

		long min = 0;
		long.TryParse(reader.Value.ToString(), out min);
		m_min = (float)BitConverter.Int64BitsToDouble(min);

		reader.Read();
		reader.Read();

		long max = 0;
		long.TryParse(reader.Value.ToString(), out max);
		m_max = (float)BitConverter.Int64BitsToDouble(max);
	}

	public float Value
	{
		get { return m_value; }
		set { m_value = value; }
	}

	[SerializeField]
	protected float m_min = 0.0f;

	[SerializeField]
	protected float m_max = 0.0f;
}

//////////////////////////////////////////////////////////////////////////////////
/// BoolSetting
//////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class BoolSetting : Setting
{
	public BoolSetting() { }

	public BoolSetting(string key, bool value)
	{
		m_key = key;
		m_value = value;
	}

#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		bool oldValue = m_value;

		EditorGUILayout.BeginHorizontal();

		GUILayout.Label(Key, GUILayout.MaxWidth(Settings.labelWidth));
		m_value = EditorGUILayout.Toggle(m_value);

		EditorGUILayout.EndHorizontal();

		if (m_value != oldValue) 
		{
			FireSettingChanged();
			Settings.Instance.SaveSettings(); 
		}
	}
#endif

	public override void Serialise(JsonWriter writer)
	{
		writer.WritePropertyName("key");
		writer.WriteValue(m_key);

		writer.WritePropertyName("value");
		writer.WriteValue(m_value ? "true" : "false");
	}

	public override void Deserialise(JsonReader reader)
	{
		reader.Read();
		reader.Read();

		m_key = reader.Value.ToString();

		reader.Read();
		reader.Read();

		// Read the key
		m_value = reader.Value.ToString() == "true" ? true : false;
	}

	public bool Value
	{
		get { return m_value; }
		set { m_value = value; }
	}

	[SerializeField]
	protected bool m_value = false;
}
