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

			writer.WritePropertyName("action_id");
			writer.WriteValue(input.DataID);

			writer.WritePropertyName("blackboard_id");
			writer.WriteValue(input.BlackboardSourceID == null ? "" : input.BlackboardSourceID);

			writer.WritePropertyName("data_type");
			writer.WriteValue(input.DataType);

			writer.WriteEndObject();
		}

		writer.WriteEndArray();

		writer.WritePropertyName("output_data");
		writer.WriteStartArray();

		foreach (var input in m_outputData)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("action_id");
			writer.WriteValue(input.DataID);

			writer.WritePropertyName("blackboard_id");
			writer.WriteValue(input.BlackboardSourceID == null ? "" : input.BlackboardSourceID);

			writer.WritePropertyName("data_type");
			writer.WriteValue(input.DataType);

			writer.WriteEndObject();
		}

		writer.WriteEndArray();

		// Links
		writer.WritePropertyName("links");
		writer.WriteStartArray();

		for (int i = 0; i < m_outputLinks.Count; i++ )
		{
			if(m_outputLinks[i].linkAction != null)
			{
				writer.WriteStartObject();

				writer.WritePropertyName("link_name");
				writer.WriteValue(m_outputLinks[i].linkName);

				writer.WritePropertyName("link_id");
				writer.WriteValue(m_outputLinks[i].linkAction.SerialisationID);

				writer.WriteEndObject();
			}
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

				if(reader.Value.ToString() == "id")
				{
					reader.Read();
					int id = -1;
					int.TryParse(reader.Value.ToString(), out id);
					SerialisationID = id;
				}

				if(reader.Value.ToString() == "input_data")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising input data");
					DeserialiseData(reader, m_inputData);
				}
				else if(reader.Value.ToString() == "output_data")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising output data");
					DeserialiseData(reader, m_outputData);
				}
				else if(reader.Value.ToString() == "links")
				{
					reader.Read();
					Debug.Log("\t\tDeserialising links");
					DeserialiseLinks(reader);
				}
				else
				{
					reader.Read();
				}
			}
			else
			{
				reader.Read();
			}

		}
	}

	private void DeserialiseData(JsonReader reader, List<AIActionData> dataList)
	{
		int previousDepth = reader.Depth;
		reader.Read();
		int depth = reader.Depth;

		if(depth == previousDepth)
		{
			return;
		}

		bool dataFound = false;
		string currentDataID = string.Empty;
		
		while(reader.Depth >= depth)
		{
			if(reader.TokenType == JsonToken.PropertyName)
			{
				if(reader.Value.ToString() == "blackboard_id")
				{
					reader.Read();
					bool found = false;
					for(int i = 0; i < dataList.Count; i++)
					{
						if(dataList[i].DataID == currentDataID)
						{
							var data = dataList[i];
							data.BlackboardSourceID = reader.Value == null ? "" : reader.Value.ToString();
							dataList[i] = data;
							found = true;
						}
					}
					if(!found)
					{
						Debug.Break();
					}
				}
				else if(reader.Value.ToString() == "action_id")
				{
					reader.Read();
					currentDataID = reader.Value.ToString();
				}
			}
			reader.Read();
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
		Debug.Log("<color=blue> Deserialised " + m_linkSerialisationMap.Count + " Links</color>");
	}

	public void PostDeserialise()
	{
		foreach(var link in m_linkSerialisationMap)
		{
			AIAction linkAction = Task.GetActionFromSerialisationID(link.Value);

			if(linkAction != null)
			{
				Debug.Log("\t\tAdding link to <b>" + linkAction.Name + "</b> from <b>" + Name + "</b>");

				for (int index = 0; index < m_outputLinks.Count; index++ )
				{
					if (m_outputLinks[index].linkName == link.Key)
					{
						var targetLink = m_outputLinks[index];
						targetLink.linkAction = linkAction;
						m_outputLinks[index] = targetLink;
					}
				}
			}
			Debug.Log("<color=purple> Complete " + m_outputLinks.Count + " links</color>");
		}

		foreach (var inputData in m_inputData)	{ m_inputDataRects.Add(new Rect());	}
		foreach (var outputData in m_outputData) { m_outputDataRects.Add(new Rect()); }
	}

	public Dictionary<string, int> m_linkSerialisationMap = new Dictionary<string, int>();
}
