/// <summary>
/// 
/// Serialiser.cs
/// 
/// Controls writing of custom save-data.
/// 
/// </summary>

using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;

public class Serialiser 
{
	public static Serialiser Instance
	{
		get
		{
			if(s_instance == null)
			{
				s_instance = new Serialiser();	
			}
			
			return s_instance;
		}
	}

	private Serialiser() {}
	
	public void Serialise()
	{
		UnityEngine.Object[] serialisableObjects = GameObject.FindObjectsOfType(typeof(SerialisableObject));
		
		if(OutputDebugInfo)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Serialising ").Append(serialisableObjects.Length).Append(" objects");
			Debug.Log(builder.ToString());
			Debug.Log(Directory.GetCurrentDirectory());
		}
		
		XmlTextWriter writer 	= new XmlTextWriter("testSave.xml", Encoding.UTF8);
		writer.Formatting 		= Formatting.Indented;
		
		writer.WriteStartDocument();
		writer.WriteStartElement("entries");
		
		for(int objectID = 0; objectID < serialisableObjects.Length; ++objectID)
		{
			SerialiseObject(writer, serialisableObjects[objectID] as SerialisableObject);
		}
		
		writer.WriteEndElement();
		writer.WriteEndDocument();
		writer.Close();
	}
	
	public void Deserialise()
	{
		Dictionary<string, SerialisableObject> objects 	= new Dictionary<string, SerialisableObject>();
		SerialisableObject[] serialisableObjects 		= GameObject.FindObjectsOfTypeAll(typeof(SerialisableObject)) as SerialisableObject[];
		
		// Create a dictionary of all the serialisable objects in the scene, to prevent iteration later.
		foreach(var currentObject in serialisableObjects)
		{
			objects.Add(currentObject.GUID, currentObject);
		}
		
		XmlDocument saveFile = new XmlDocument();
		saveFile.Load("testSave.xml");
		
		XmlNodeList entries = saveFile.GetElementsByTagName("entry");
		
		for(int entryID = 0; entryID < entries.Count; ++entryID)
		{
			List<SavePair> pairs = new List<SavePair>();
			
			XmlNode entryNode = entries.Item(entryID);
			
			string guid = entryNode.Attributes.GetNamedItem("id").Value;
			
			for(int childID = 0; childID < entryNode.ChildNodes.Count; ++childID)
			{
				string id 		= entryNode.ChildNodes[childID].Attributes.GetNamedItem("id").Value;
				string value 	= entryNode.ChildNodes[childID].Attributes.GetNamedItem("value").Value;
				pairs.Add(new SavePair(id, value));
			}
			
			SerialisableObject targetObject = null;
			objects.TryGetValue(guid, out targetObject);
			
			if(targetObject != null)
			{
				targetObject.Deserialise(pairs);
			}
			else if(OutputDebugInfo)
			{
				Debug.Log("Dead ID found: " + guid.ToString());	
			}
		}
	}
	
	private void SerialiseObject(XmlTextWriter writer, SerialisableObject serialisableObject)
	{
		List<SavePair> pairs = new List<SavePair>();
		
		writer.WriteStartElement("entry");
		writer.WriteAttributeString("id", serialisableObject.GUID.ToString());
		
		// Dump the object-name into the save file if debugging
		if(OutputDebugInfo)
		{
			writer.WriteAttributeString("debug_name", serialisableObject.name);
		}
		
		serialisableObject.Serialise(pairs);
		
		for(int pairID = 0; pairID < pairs.Count; ++pairID)
		{
			writer.WriteStartElement("pair");
				
			writer.WriteAttributeString("id", pairs[pairID].id);
			writer.WriteAttributeString("value", pairs[pairID].value);
			
			writer.WriteEndElement();
		}
		
		writer.WriteEndElement();
	}
	
	public bool OutputDebugInfo { get; set; }
	
	private static Serialiser s_instance = null;
}
