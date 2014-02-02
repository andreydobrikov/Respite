///////////////////////////////////////////////////////////
// 
// BlackBoardEntry.cs
//
// What it does: An entry stored in an AIBlackboard
//
// Notes:   Each entry should be managed by the blackboard itself.
// 
// To-do:
//
///////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System.Collections;

public class AIBlackBoardEntry
{
    public AIBlackBoardEntry(string entryName, System.Object entryObject)
    {
        m_entryName     = entryName;
        m_entryObject   = entryObject;
    }

    public T GetObject<T>()
    {
        return (T)m_entryObject;
    }

    public void SetObject<T>(T newObject)
    {
        m_entryObject = newObject;
    }

    public string EntryName             { get { return m_entryName; } }
    public System.Object EntryObject    { get { return m_entryObject; } }

    private string m_entryName           = string.Empty;
    private System.Object m_entryObject  = null;
}