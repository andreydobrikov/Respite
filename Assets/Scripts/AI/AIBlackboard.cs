/// <summary>
/// AI blackboard.
/// 
/// Just a big dumping ground for all the shared data the AI behaviours may want to fiddle with.
/// It seemed to be asking for trouble to keep this data in the AI class.
/// 
/// </summary>

#define AI_OUTPUT
#define AI_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBlackboard 
{
	// Adds a new blackboard entry and returns its key for later retrieval
	public int AddEntry<T>(string entryName, T entryObject)
	{
		int nameHash = entryName.GetHashCode();

		if(!m_blackboardEntries.ContainsKey(nameHash))
		{
			BlackBoardEntry newEntry = new BlackBoardEntry();

			newEntry.EntryName 		= entryName;
			newEntry.EntryObject 	= (System.Object)entryObject;

			m_blackboardEntries.Add(nameHash, newEntry);
		}
#if AI_OUTPUT
		else
		{
			Debug.LogWarning("\"" + entryName + "\" already added.");
		}
#endif

		return nameHash;
	}

	/// <summary>
	/// Retrieves a blackboard entry, returning true if the entry is found, false if not.
	/// </summary>
	public bool GetEntry<T>(int entryHash, ref T outVal)
	{
		BlackBoardEntry outputEntry;

		if(m_blackboardEntries.TryGetValue(entryHash, out outputEntry))
		{
			outVal = (T)(outputEntry.EntryObject);
			return true;
		}

#if AI_OUTPUT
		Debug.LogWarning("Requested unknown entry with hash " + entryHash);
#endif

		return false;
	}

	/// <summary>
	/// Sets the entry.
	/// </summary>
	public bool SetEntry<T>(int entryHash, T newValue)
	{
		BlackBoardEntry outputEntry;
		
		if(m_blackboardEntries.TryGetValue(entryHash, out outputEntry))
		{
#if AI_DEBUG
			if(newValue.GetType() != outputEntry.EntryObject.GetType())
			{
				Debug.LogError("Assigning mis-matched type for entry \"" + outputEntry.EntryName + "\"!\nCurrent type is \"" + outputEntry.EntryObject.GetType().Name + "\". New type is \"" + newValue.GetType().Name + "\"");
			}
#endif
			outputEntry.EntryObject = newValue;
			return true;
		}

#if AI_OUTPUT
		Debug.LogWarning("Attempting to set unknown entry with hash " + entryHash);
#endif

		return false;
	}

	private Dictionary<int, BlackBoardEntry> m_blackboardEntries = new Dictionary<int, BlackBoardEntry>();

}

public struct BlackBoardEntry
{
	public string EntryName;
	public System.Object EntryObject;
}
