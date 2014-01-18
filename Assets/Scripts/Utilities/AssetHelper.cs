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

		UnityEngine.Object asset = null;

#if UNITY_EDITOR
		 asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
#endif

		if(asset == null)
		{
			string extension = System.IO.Path.GetExtension(assetPath);
			assetPath = assetPath.Substring(0, assetPath.Length - extension.Length);
			
			asset = Resources.Load(assetPath);
		}


		if(asset == null)
		{
			Debug.LogError("Failed to load asset: " + assetPath);	
		}
		
		return asset;
	}
	
	public UnityEngine.Object FindAsset<T>(string partialName)
	{
		UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(T));
		
		foreach(var current in objects)
		{
			if(current.name.Contains(partialName))
			{
				return current;
			}
		}

		T[] allAssets = GetAssetsAtPath<T>("");

		foreach(var asset in allAssets)
		{
			Object assetObject = asset as Object;
			if(assetObject.name.Contains(partialName))
			{
				return asset as UnityEngine.Object;
			}
		}
		
		return null;
	}

	public static T[] GetAssetsAtPath<T> (string path) 
	{
		ArrayList al = new ArrayList();
		
		string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
		
		foreach(string fileName in fileEntries)
		{
			int index = fileName.LastIndexOf("/");
			
			string localPath = "Assets/" + path;
			
			if (index > 0)
			{
				localPath += fileName.Substring(index);
			}
			
			Object t = Resources.LoadAssetAtPath(localPath, typeof(T));
			
			if(t != null)
			{
				al.Add(t);
			}
			
		}
		
		T[] result = new T[al.Count];
		
		for(int i=0;i<al.Count;i++)
		{
			result[i] = (T)al[i];
		}
		
		return result;
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
		int dataPathIndex = path.IndexOf(Application.dataPath) ;
		if(dataPathIndex != -1)
		{
			path = path.Remove(dataPathIndex, Application.dataPath.Length + 1);
			path = "assets/" + path;
		}

		return path;
	}
	
	private AssetHelper() {}
	
	private static AssetHelper s_instance = null;
}
