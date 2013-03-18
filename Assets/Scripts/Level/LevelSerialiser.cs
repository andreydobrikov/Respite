using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;
/*
public partial class Level : MonoBehaviour 
{
	public void Serialise(string path)
	{
		FileInfo info = new FileInfo(path);
		if(info.Exists && info.IsReadOnly)
		{
			Debug.LogError("File is read-only: " + path);	
			return;
		}
		
		int width = m_layout.GetLength(0);
		int height = m_layout.GetLength(1);
		
		using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.Unicode))
		{
			writer.Formatting = Formatting.Indented;
 			writer.WriteStartDocument();
			
			writer.WriteStartElement("level");
			
			writer.WriteAttributeString("width", width.ToString());
			writer.WriteAttributeString("height", height.ToString());
			
			writer.WriteStartElement("tiles");
			
			for(int x = 0; x < width; ++x)
			{
				for(int y = 0; y < height; ++y)
				{
					writer.WriteStartElement("tile");
					
					writer.WriteAttributeString("x", x.ToString());
					writer.WriteAttributeString("y", y.ToString());
					writer.WriteAttributeString("blocked", m_layout[x, y].ToString());
					
					writer.WriteEndElement();
				}
			}
			
			writer.WriteEndElement();
			
			writer.WriteEndElement();
			
			writer.WriteEndDocument();
		}
	}
	
	/// <summary>
	/// Deserialise the specified path.
	/// </summary>
	/// <param name='path'>
	/// Asset-directory local path
	/// </param>
	public void Deserialise(string path)
	{
		string absolutePath = Application.dataPath + "\\" + path;
		
		Debug.Log("Deserialising Level: " + absolutePath);
		FileInfo info = new FileInfo(absolutePath);
		if(info.Exists)
		{
			XmlDocument document = new XmlDocument();
			document.Load(absolutePath);
			
			XmlNodeList levelElement = document.GetElementsByTagName("level");
			
			XmlNode widthNode = levelElement[0].Attributes.GetNamedItem("width");
			XmlNode heightNode = levelElement[0].Attributes.GetNamedItem("height");
			
			string widthValue = widthNode.Value;
			string heightValue = heightNode.Value;
			
			
			int.TryParse(widthValue, out Width);
			int.TryParse(heightValue, out Height);
			
			m_layout = new bool[Width, Height];
			
			// Parse the tiles
			XmlNodeList tiles = document.GetElementsByTagName("tile");
			
			foreach(XmlNode node in tiles)
			{
				string xValue = node.Attributes.GetNamedItem("x").Value;	
				string yValue = node.Attributes.GetNamedItem("y").Value;
				string blockedValue = node.Attributes.GetNamedItem("blocked").Value;
				
				int x = 0;
				int y = 0;
				bool blocked = false;
				
				if(int.TryParse(xValue, out x) && int.TryParse(yValue, out y) && bool.TryParse(blockedValue, out blocked))
				{
					m_layout[x, y] = blocked;
				}
				else
				{
					Debug.LogError("Parse error: " + xValue + ", " + yValue + " | " + blockedValue);	
				}
			}
			
			m_loadedLevel = path;
			Loaded = true;
			
			// Flag the level as dirty so dependent systems know to refresh
			m_dirty = true;
		}
		else
		{
			Debug.LogError("Failed to open file: " + path);
		}
		
	}
}
 */
			