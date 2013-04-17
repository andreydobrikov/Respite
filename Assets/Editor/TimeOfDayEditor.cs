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
			EditorGUILayout.BeginHorizontal();
			
			
			float newTime = GUILayout.HorizontalSlider(timeOfDay.ActiveTime, 0.0f, 1.0f);
			newTime = EditorGUILayout.FloatField(newTime);
			
			EditorGUILayout.EndHorizontal();
			
			if(newTime != timeOfDay.ActiveTime)
			{
				timeOfDay.ActiveTime = 	newTime;
				timeOfDay.UpdateTime(true);
				EditorUtility.SetDirty (timeOfDay);
			}
		
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
		}
	}
}
