#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SerialisableObject : MonoBehaviour 
{
	public string GUID = null;
	
	void OnEnable()
	{
		if(string.IsNullOrEmpty(GUID))
		{
			GUID = System.Guid.NewGuid().ToString();	
		}
	}
	
	public void Serialise(List<SavePair> pairs)
	{
		SendMessage("SaveSerialise", pairs, SendMessageOptions.DontRequireReceiver);
	}
	
	public void Deserialise(List<SavePair> pairs)
	{
		SendMessage("SaveDeserialise", pairs, SendMessageOptions.DontRequireReceiver);
	}
	
	public void AssignNewID()
	{
		GUID = System.Guid.NewGuid().ToString();
	}
	
#if UNITY_EDITOR
	[MenuItem ("Respite/Save System/Generate New IDs")]
	static void RebuildIDs () 
	{
		bool rebuildIDs = EditorUtility.DisplayDialog("Rebuild all IDs?", "This will invalidate existing saves.", "yes", "no");
		
		SerialisableObject[] serialisableObjects = FindObjectsOfType (typeof(SerialisableObject)) as SerialisableObject[];
		
		foreach(var currentObject in serialisableObjects)
		{
			currentObject.AssignNewID();
		}
		
		Debug.Log("New IDs Generated");
	}
#endif
}

public class SavePair
{
	public SavePair(string id, string value)
	{
		this.id 	= id;
		this.value 	= value;
	}
	
	public string id;
	public string value;
}
