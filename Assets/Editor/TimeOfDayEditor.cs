using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
			
			List<TODKeyFrame> toDelete = new List<TODKeyFrame>();
			bool resort = false;
			
			foreach(var frame in timeOfDay.Frames)
			{
				EditorGUILayout.BeginHorizontal();
				
				Vector4 newColor = EditorGUILayout.ColorField(frame.FrameColor);
				float newFrameTime = EditorGUILayout.FloatField("Time", frame.FrameTime);	
				
				if(newFrameTime != frame.FrameTime)
				{
					frame.FrameTime = newFrameTime;
					resort = true;
				}
				
				if(GUILayout.Button("Delete"))
				{
					toDelete.Add(frame);	
				}
				
				if(newColor != frame.FrameColor)
				{
					frame.FrameColor = newColor;
				}
				
				timeOfDay.UpdateTime(true);
				EditorUtility.SetDirty (timeOfDay);
				
				EditorGUILayout.EndHorizontal();
			}
			
			if(resort)
			{
				timeOfDay.Frames.Sort();
			}
			
			foreach(var frame in toDelete)
			{
				timeOfDay.Frames.Remove(frame);	
			}
			
			if(GUILayout.Button("Add Keyframe"))
			{
				timeOfDay.Frames.Add(new TODKeyFrame());
				timeOfDay.Frames.Sort();
				timeOfDay.UpdateTime(true);
				EditorUtility.SetDirty (timeOfDay);
			}
			
			EditorGUILayout.EndVertical();
		}
		else
		{
			GUILayout.Label("Current Time: " + timeOfDay.AdjustedTime);
			timeOfDay.PauseUpdate = GUILayout.Toggle(timeOfDay.PauseUpdate, "Pause");
			
			float newTime = GUILayout.HorizontalSlider(timeOfDay.AdjustedTime, 0.0f, 1.0f);
			newTime = EditorGUILayout.FloatField(newTime, GUILayout.Width(80));
			
			timeOfDay.AdjustedTime = newTime;
		}
	}
}
