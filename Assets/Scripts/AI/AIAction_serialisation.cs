using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public abstract partial class AIAction
{
	public void Serialise(JsonWriter writer)
	{
		writer.WritePropertyName("action_type");
		writer.WriteValue(GetType().AssemblyQualifiedName);

		writer.WritePropertyName("id");
		writer.WriteValue(SerialisationID);

		// Output Data
		writer.WritePropertyName("input_data");
		writer.WriteStartArray();

		foreach(var input in m_inputData)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("blackboard_id");
			writer.WriteValue(input.BlackBoardID);

			writer.WritePropertyName("action_id");
			writer.WriteValue(input.ActionID);

			writer.WritePropertyName("data_type");
			writer.WriteValue(input.DataType.AssemblyQualifiedName);

			writer.WriteEndObject();
		}

		writer.WriteEndArray();

		// Links
		writer.WritePropertyName("links");
		writer.WriteStartArray();

		foreach(var link in m_links)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("link_name");
			writer.WriteValue(link.Key);

			writer.WritePropertyName("link_id");
			writer.WriteValue(link.Value.SerialisationID);

			writer.WriteEndObject();
		}

		writer.WriteEndArray();
	}

	public void Deserialise(JsonReader reader)
	{
		int depth = reader.Depth;

		while(reader.Depth >= depth)
		{
			if(reader.TokenType == JsonToken.PropertyName)
			{
				Debug.Log("<color=green>" + reader.Value.ToString() + "</color>");
				if(reader.Value.ToString() == "input_data")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising input data");
					DeserialiseData(reader, m_inputData);
				}
				else if(reader.Value.ToString() == "output_data")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising input data");
					DeserialiseData(reader, m_outputData);
				}
				else if(reader.Value.ToString() == "links")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising links");
					DeserialiseLinks(reader);
				}

			}
			reader.Read();
		}
	}

	private void DeserialiseData(JsonReader reader, List<AIActionData> dataList)
	{
		reader.Read();
		int depth = reader.Depth;

		bool dataFound = false;
		AIActionData currentData = new AIActionData();
		
		while(reader.Depth >= depth)
		{
			if(reader.TokenType == JsonToken.PropertyName)
			{
				if(reader.Value.ToString() == "blackboard_id")
				{
					if(dataFound)
					{
						Debug.Log("\t\t\tAdding data with blackboard_id: <b>" + currentData.BlackBoardID + "</b>");
						dataList.Add(currentData);
					}

					reader.Read();

					currentData = new AIActionData();
					currentData.BlackBoardID = reader.Value.ToString();

					dataFound = true;
				}
				else if(reader.Value.ToString() == "action_id")
				{
					reader.Read();
					currentData.ActionID = reader.Value.ToString();
				}
				else if(reader.Value.ToString() == "data_type")
				{
					reader.Read();

					Type newType = Type.GetType(reader.Value.ToString());
					currentData.DataType = newType;
				}

			}
			reader.Read();
		}

		if(dataFound)
		{
			Debug.Log("\t\t\tAdding data with blackboard_id: <b>" + currentData.BlackBoardID + "</b>");
			dataList.Add(currentData);
		}
	}

	private void DeserialiseLinks(JsonReader reader)
	{
		reader.Read();
		int depth = reader.Depth;

		string linkName = string.Empty;
		int linkID		= -1;

		while(reader.Depth >= depth)
		{
			if(reader.TokenType == JsonToken.PropertyName)
			{
				if(reader.Value.ToString() == "link_name")
				{
					reader.Read();
					linkName = reader.Value.ToString();
				}
				else if(reader.Value.ToString() == "link_id")
				{
					reader.Read();
					if(int.TryParse(reader.Value.ToString(), out linkID))
					{
						Debug.Log("\t\tLink found");
						m_linkSerialisationMap.Add(linkName, linkID);
					}
				}
			}
			reader.Read();
		}
	}

	public void PostDeserialise()
	{
		foreach(var link in m_linkSerialisationMap)
		{
			AIAction linkAction = Task.GetActionFromSerialisationID(link.Value);

			if(linkAction != null)
			{
				Debug.Log("\t\tAdding link to <b>" + linkAction.Name + "</b> from <b>" + Name + "</b>");
				m_links.Add(link.Key, linkAction);
			}
		}
	}

	public Dictionary<string, int> m_linkSerialisationMap = new Dictionary<string, int>();
}
