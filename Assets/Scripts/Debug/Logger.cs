// Logs extra debug data during loading and saving
#define AI_LOGGING					

// Enables exception handling in the OccludedMesh class. Largely useless.
#define OCCLUDED_MESH_EXCEPTIONS

using System.Collections.Generic;
using UnityEngine;

public enum LogChannel
{
	Default = -1,
	AI,
	Audio,
	Count
}

public class Logger : MonoBehaviour
{
	void Start()
	{
		for (int i = ((int)LogChannel.Default) + 1; i < (int)LogChannel.Count; i++)
		{
			BoolSetting channelSetting = Settings.Instance.GetSetting<BoolSetting>("debug_" + System.Enum.GetName(typeof(LogChannel), (LogChannel)i) );

			m_enabledChannels[i] = new ChannelInfo();
			m_enabledChannels[i].channel = (LogChannel)i;
			m_enabledChannels[i].enabled = channelSetting != null ? channelSetting.Value : false;
			m_enabledChannels[i].color	 = i < s_channelColors.Length ? s_channelColors[i] : "white";
			m_enabledChannels[i].channelPrefix = System.Enum.GetName(typeof(LogChannel), (LogChannel)i);

			if (channelSetting != null)
			{
				channelSetting.AddChangedCallback(m_enabledChannels[i].SettingChanged);
			}
		}
	}

	public static void Log(string message)			{ Debug.Log(message); }
	public static void LogWarning(string message)	{ Debug.LogWarning(message); }
	public static void LogError(string message)		{ Debug.LogError(message); }

	public static void Log(string message, LogChannel category)
	{
		int channelIndex = (int)category;

		if(category == LogChannel.Default || category == LogChannel.Count) { Debug.LogError("Invalid message channel used."); }
		if (!m_enabledChannels[channelIndex].enabled) { return; }

		string finalMessage = "<color=\"" + m_enabledChannels[channelIndex].color + "\">" + "(" + m_enabledChannels[channelIndex].channelPrefix + ") " + message + "</color>";
		Debug.Log(finalMessage);
	}

	public static void LogWarning(string message, LogChannel category)
	{
		int channelIndex = (int)category;

		if (category == LogChannel.Default || category == LogChannel.Count) { Debug.LogError("Invalid message channel used."); }
		if (!m_enabledChannels[channelIndex].enabled) { return; }

		string finalMessage = "<color=\"" + m_enabledChannels[channelIndex].color + "\">" + "(" + m_enabledChannels[channelIndex].channelPrefix + ") " + message + "</color>";
		Debug.LogWarning(finalMessage);
	}

	public static void LogError(string message, LogChannel category)
	{
		int channelIndex = (int)category;

		if (category == LogChannel.Default || category == LogChannel.Count) { Debug.LogError("Invalid message channel used."); }
		if (!m_enabledChannels[channelIndex].enabled) { return; }

		string finalMessage = "<color=\"" + m_enabledChannels[channelIndex].color + "\">" + "(" + m_enabledChannels[channelIndex].channelPrefix + ") " + message + "</color>";
		Debug.LogError(finalMessage);
	}

	private class ChannelInfo
	{
		public LogChannel channel;
		public bool enabled;
		public string color;
		public string channelPrefix;

		public void SettingChanged()
		{
			BoolSetting channelSetting = Settings.Instance.GetSetting<BoolSetting>("debug_" + channelPrefix);
			if (channelSetting != null)
			{
				enabled = channelSetting.Value;
			}
		}
	}

	private static string[] s_channelColors = 
	{ 
		"green",		// AI
		"orange"		// Audio
	};

	private static ChannelInfo[] m_enabledChannels = new ChannelInfo[(int)LogChannel.Count];
}