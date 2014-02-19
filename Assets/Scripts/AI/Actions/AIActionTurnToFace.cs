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
	private AIActionData m_targetData	= null;
	private Vector3 m_target			= Vector3.zero;

    public AIActionTurnToFace()
    {
        m_name = "Turn To Face";
    }

    public override void Init()
    {
        AddOutputLink("turn_complete");

		m_targetData = ScriptableObject.CreateInstance(typeof(AIActionData)) as AIActionData;

		m_targetData.DataID = "turn_to_face_target";
		m_targetData.DataType = typeof(Vector3).AssemblyQualifiedName;
        
        // Add data
		m_inputData.Add(m_targetData);
    }

    public override void Start()
    {
		GetBlackboardData<Vector3>(m_targetData.BlackboardSourceID, ref m_target);
		m_result = AIActionResult.Running;
    }

    public override void Update()
    {
		Debug.DrawLine(GetGameObject().transform.position, m_target, Color.yellow);
    }

    public override void Stop()
    {
        
    }

	
}
