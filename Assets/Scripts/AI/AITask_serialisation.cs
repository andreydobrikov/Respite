using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public sealed partial class AITask : IComparable 
{
	public void Serialise(string directory)
	{ 
		int ID = 0;
		AssignSerialisationIDs(ref ID);

		string path = directory + "/" + Name + ".json";

		if(!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}

		using (StreamWriter sw = new StreamWriter(path))
		{
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.Indented;

				writer.WriteStartObject();

				writer.WritePropertyName("name");
				writer.WriteValue(Name);

				writer.WritePropertyName("actions");
				writer.WriteStartArray();
				foreach(var action in m_actions) 
				{
					writer.WriteStartObject();
					action.Serialise(writer);
					writer.WriteEndObject();
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
		}
	}

	public bool Deserialise(string jsonSource)
	{
		m_actions.Clear();

		try
		{
			using(TextReader tr = new StringReader(jsonSource))  
			{
				using(JsonReader reader = new JsonTextReader(tr)) 
				{
					while(reader.Read())
					{
						if(reader.TokenType == JsonToken.PropertyName)
						{
							if(reader.Value.ToString() == "name")
							{
								reader.Read();
								Name = reader.Value.ToString();
							}
							
							if(reader.Value.ToString() == "actions")
							{
								
								DeserialiseActions(reader);
							}
						}
					}
				}
			}
		}
		catch( Exception e)
		{
			Debug.LogError("Failed to load JSON:\n" + jsonSource + "\n" + e.ToString());
			return false;
		}


		return true;
	}

	private void DeserialiseActions(JsonReader reader)
	{
		reader.Read();
		int depth = reader.Depth;

		// While still in the actions block
		while(reader.Depth >= depth)
		{
			if(reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "action_type")
			{
				reader.Read();

				Type actionType = Type.GetType(reader.Value.ToString());
				AIAction newAction = ScriptableObject.CreateInstance(actionType) as AIAction;
				newAction.Task = this;

				if(newAction == null)
				{
					Debug.LogWarning("\tFailed to instantiate type: " + reader.Value.ToString());
				}
				else
				{
					newAction.Init();
					newAction.Deserialise(reader);
					m_actions.Add(newAction);
				}
			}

			reader.Read();
		}

		foreach(var action in m_actions)
		{
			action.PostDeserialise();
		}
	}

	// Ensures each child has a unique ID for referencing during serialisation
	private void AssignSerialisationIDs(ref int ID)
	{
		foreach(var action in m_actions)
		{
			action.SerialisationID = ID;
			ID++;
		}
	}
}
