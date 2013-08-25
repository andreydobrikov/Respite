///////////////////////////////////////////////////////////
// 
// GUIManager.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GUIManager : MonoBehaviour 
{
	public GUISkin guiSkin;
	 
	private static GUIManager instance;
	 
	void OnEnable()
	{
	    instance = this;
	}
	 
	public static GUISkin GetSkin() 
	{
		return instance.guiSkin;
	}
	
	public static void SetSkin(GUISkin skin)
	{
		instance.guiSkin = skin;
	}
}
