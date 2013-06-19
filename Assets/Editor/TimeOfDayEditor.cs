using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TimeOfDay))]
public class TimeOfDayEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		TimeOfDay timeOfDay = (TimeOfDay)target;
		
		if(!Application.isPlaying)
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
			
			GUILayout.Label("Start Time", GUILayout.Width(80));
			
			float newStartTime = GUILayout.HorizontalSlider(timeOfDay.StartTime, 0.0f, 1.0f);
			newStartTime = EditorGUILayout.FloatField(newStartTime, GUILayout.Width(80));
			
			timeOfDay.StartTime = newStartTime;
			
			EditorGUILayout.EndHorizontal();
		
			EditorGUILayout.BeginHorizontal();
			
			GUILayout.Label("Current Time", GUILayout.Width(80));
			float newTime = GUILayout.HorizontalSlider(timeOfDay.ActiveTime, 0.0f, 1.0f);
			newTime = EditorGUILayout.FloatField(newTime, GUILayout.Width(80));
			
			EditorGUILayout.EndHorizontal();
			
			if(newTime != timeOfDay.ActiveTime)
			{
				timeOfDay.ActiveTime = 	newTime;
				timeOfDay.UpdateTime(true);
				EditorUtility.SetDirty (timeOfDay);
			}
			
			timeOfDay.CloudCoverPercentage = EditorGUILayout.FloatField("Cloud Cover", timeOfDay.CloudCoverPercentage);
			
			EditorGUILayout.EndVertical();
		
			EditorGUILayout.BeginVertical();
			
			foreach(var frame in timeOfDay.Frames)
			{
				EditorGUILayout.BeginHorizontal();
				
				Vector4 newColor = EditorGUILayout.ColorField(frame.FrameColor);
				frame.FrameTime = EditorGUILayout.FloatField("Time", frame.FrameTime);	
				
				if(newColor != frame.FrameColor)
				{
					frame.FrameColor = newColor;
				}
				
				timeOfDay.UpdateTime(true);
				EditorUtility.SetDirty (timeOfDay);
				
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.EndVertical();
		}
		else
		{
			GUILayout.Label("Current Time: " + timeOfDay.AdjustedTime);
			timeOfDay.PauseUpdate = GUILayout.Toggle(timeOfDay.PauseUpdate, "Pause");
		}
	}
}
