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
