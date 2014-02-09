///////////////////////////////////////////////////////////
// 
// AIActionTurnToFace.cs
//
// What it does: Rotates the entity to face a given point.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class AIActionTurnToFace : AIAction 
{
    public AIActionTurnToFace()
    {
        m_name = "Turn To Face";
    }

    public override void Init()
    {
        AddOutputLink("turn_complete");

        AIActionData navTargetData = ScriptableObject.CreateInstance(typeof(AIActionData)) as AIActionData;
        
        navTargetData.DataID = "nav_target";
        navTargetData.DataType = typeof(Vector3).AssemblyQualifiedName;
        
        // Add data
        m_inputData.Add(navTargetData);
    }

    public override void Start()
    {
        
    }

    public override void Update()
    {
        
    }

    public override void Stop()
    {
        
    }
}
