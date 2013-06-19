using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;

public class AssetHelper 
{
	public UnityEngine.Object GetAsset<T>(string assetPath)
	{
		assetPath = StripResourcePath(assetPath);
		
		string extension = System.IO.Path.GetExtension(assetPath);
		assetPath = assetPath.Substring(0, assetPath.Length - extension.Length);
		
		UnityEngine.Object asset = Resources.Load(assetPath);
		
		if(asset == null)
		{
//			Debug.LogError("Failed to load asset: " + assetPath);	
		}
		
		return asset;
	}
	
	public static AssetHelper Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new AssetHelper();	
			}
			
			return s_instance;
		}
	}
	
	public static string StripResourcePath(string path)
	{
		// Strip the application data-path
		int assetPathIndex = path.IndexOf(Application.dataPath + "/Resources");
		if(assetPathIndex != -1)
		{
			path = path.Remove(assetPathIndex, (Application.dataPath + "/Resources").Length + 1);
		}
		
		// For projects not using the resources folder
		int dataPathIndex = path.IndexOf(Application.dataPath);
		if(dataPathIndex != -1)
		{
			path = path.Remove(dataPathIndex, Application.dataPath.Length);
		}
		
		return path;
	}
	
	private AssetHelper() {}
	
	private static AssetHelper s_instance = null;
}
