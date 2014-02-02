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
    // Structs 

	public AIBlackboard()
	{
		Initialised = false;
	}

	// Adds a new blackboard entry and returns its key for later retrieval
	public AIBlackBoardEntry AddEntry<T>(string entryName, T entryObject)
	{
		if (Initialised)
		{
			Debug.LogWarning("Registering entry \"" + entryName + "\" after initialisation is complete!");
		}

		int nameHash = entryName.GetHashCode();
        AIBlackBoardEntry entry = null;

		if(!m_blackboardEntries.TryGetValue(nameHash, out entry))
		{
            entry = new AIBlackBoardEntry(entryName, (System.Object)entryObject);
            m_blackboardEntries.Add(nameHash, entry);
		}
#if AI_OUTPUT
		else
		{
			Debug.LogWarning("\"" + entryName + "\" already added.");
		}
#endif

        return entry;
	}

	/// <summary>
	/// Retrieves a blackboard entry, returning true if the entry is found, false if not.
	/// </summary>
	public bool GetEntry<T>(int entryHash, ref T outVal)
	{
        AIBlackBoardEntry outputEntry;

		if(m_blackboardEntries.TryGetValue(entryHash, out outputEntry))
		{
			outVal = outputEntry.GetObject<T>();
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
        AIBlackBoardEntry outputEntry;
		
		if(m_blackboardEntries.TryGetValue(entryHash, out outputEntry))
		{
#if AI_DEBUG
			if(newValue.GetType() != outputEntry.GetObject<System.Object>().GetType())
			{
				Debug.LogError("Assigning mis-matched type for entry \"" + outputEntry.EntryName + "\"!\nCurrent type is \"" + outputEntry.EntryObject.GetType().Name + "\". New type is \"" + newValue.GetType().Name + "\"");
			}
#endif
			outputEntry.SetObject<T>(newValue);
			m_blackboardEntries[entryHash] = outputEntry;
			return true;
		}

#if AI_OUTPUT
		Debug.LogWarning("Attempting to set unknown entry with hash " + entryHash);
#endif

		return false;
	}

	public bool Initialised { get; set; }

    private Dictionary<int, AIBlackBoardEntry> m_blackboardEntries = new Dictionary<int, AIBlackBoardEntry>();


}

