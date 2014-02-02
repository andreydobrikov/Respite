using UnityEngine;
using System.Collections;

public class AIBehaviourSocialisation : AIBehaviour
{
    public AIBehaviourSocialisation()
    {
        m_name = "socialisation";
    }

    public override void RegisterBlackboardEntries() 
    {
        m_headTrackEntry = m_parentAI.Blackboard.AddEntry<Vector3>("headtrack_target", Vector3.forward);
    }
    
    public override void Start()
    {

    }

    public override void Update()
    {
        
    }

    public override void Shutdown()
    {
        
    }

    // Blackboard handles
    AIBlackBoardEntry m_headTrackEntry = null;
}
