using UnityEngine;
using System.Collections.Generic;

public class AIBehaviourSocialisation : AIBehaviour, IAiPerceptionListener
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
        m_parentAI.RegisterPerceptionListener(this);
    }

    public override void Update()
    {
        AIEntityMemory currentMemory = null;

        Vector3 position = m_parentAI.transform.position;

        foreach(var entity in m_parentAI.PerceivedEntitites)
        {
            // Fetch the memory of the entity.
            if(!m_memories.TryGetValue(entity.EntityID, out currentMemory))
            {
                Debug.LogError("Entity has no memory: " + entity.EntityName);
            }

            // Check whether the entity can be seen.
            float coverage = EntityUtils.SweepRadius(position, entity.transform.position, entity.GetVisibilityRadius(), 10, 1 << LayerMask.NameToLayer("LevelGeo"));
            if(coverage > 0.0f)
            {
                if(currentMemory.state != EntityMemoryState.InAwareness)
                {
                    HandleGainedAwareness(entity);
                }

                currentMemory.state                 = EntityMemoryState.InAwareness;
                currentMemory.lastAwareTime         = Time.time;
            }
            else if(currentMemory.state == EntityMemoryState.InAwareness)
            {
                // If not, check the cooldown to see if the entity has left awareness.
                // TODO: Move the cooldown to settings.
                if(Time.time - currentMemory.lastAwareTime > 3.0f)
                {
                    HandleLostAwareness(entity);
                    currentMemory.state = EntityMemoryState.InPerception;
                }
            }

            switch(currentMemory.state)
            {
                case EntityMemoryState.Idle:            { Debug.Log("Idle"); break; }
                case EntityMemoryState.InPerception:    { Debug.Log("Perception"); break; }
                case EntityMemoryState.InAwareness:     { Debug.Log("Awareness"); break; }
            }
        }
    }

    public override void Shutdown()
    {
        m_parentAI.UnregisterPerceptionListener(this);
    }

    public void EntityEnteredPerception(Entity addedEntity)
    {
        AIEntityMemory memory = null;
        // Add a new memory if this is the first time encountering the other entity
        if(!m_memories.TryGetValue(addedEntity.EntityID, out memory))
        {
            Debug.Log("Unknown entity in perception. Adding memory: " + addedEntity.EntityName);

            memory = ScriptableObject.CreateInstance<AIEntityMemory>();
            memory.id = addedEntity.EntityID;
            m_memories.Add(addedEntity.EntityID, memory);
        }

        memory.state = EntityMemoryState.InPerception;
    }

    public void EntityLeftPerception(Entity removedEntity)
    {
        AIEntityMemory memory = null;
        // Add a new memory if this is the first time encountering the other entity
        if(!m_memories.TryGetValue(removedEntity.EntityID, out memory))
        {
            Debug.LogError("Memory not found for entity: " + removedEntity.EntityName);
        }

        memory.state = EntityMemoryState.Idle;
    }

    private void HandleGainedAwareness(Entity entity)
    {

    }

    private void HandleLostAwareness(Entity entity)
    {

    }

    // Blackboard handles
    AIBlackBoardEntry m_headTrackEntry = null;
    private Dictionary<int, AIEntityMemory> m_memories = new Dictionary<int, AIEntityMemory>();
}
