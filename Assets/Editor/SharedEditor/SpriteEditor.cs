using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Sprite))] 
public class SpriteEditor : Editor
{
	
	public override void OnInspectorGUI()
	{
		Sprite sprite = (Sprite)target;
		
		GUILayout.BeginHorizontal();
		
		sprite.SpriteTexture = EditorGUILayout.ObjectField(sprite.SpriteTexture, typeof(Texture2D), false, GUILayout.Width(60), GUILayout.Height(60)) as Texture2D;
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		if(GUILayout.Button(sprite.SpriteData))
		{
			sprite.SpriteData = EditorUtility.OpenFilePanel("Open Sprite Data", "", "xml");
			sprite.SpriteData = AssetHelper.StripResourcePath(sprite.SpriteData);
		}
		
		
		sprite.updateSpeed = EditorGUILayout.FloatField("Update Speed", sprite.updateSpeed);
		sprite.BlendFrames = EditorGUILayout.Toggle("Blend Frames", sprite.BlendFrames);
		sprite.UseDebugColours = EditorGUILayout.Toggle("Use Debug Colours", sprite.UseDebugColours);
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
}
