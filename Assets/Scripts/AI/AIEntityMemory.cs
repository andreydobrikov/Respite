///////////////////////////////////////////////////////////
// 
// AIEntityMemory.cs
//
// What it does: Stores all state information relating to an entity's memory of another.
//
// Notes: Keep it small. Ha.
// 
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public enum EntityMemoryState
{
    Idle,
    InPerception,
    InAwareness
}

public class AIEntityMemory : ScriptableObject
{
    public int id                   = -1;
    public EntityMemoryState state  = EntityMemoryState.Idle;
    public float lastAwareTime      = Mathf.NegativeInfinity;
}
