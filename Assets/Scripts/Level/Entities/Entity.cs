///////////////////////////////////////////////////////////
// 
// Entity.cs
//
// What it does: Base class for entities in the world. 
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public abstract class Entity : MonoBehaviour 
{
    public void Start()
    {
        EntityStart();
        m_entityID = EntityManager.Instance.RegisterEntity(this);
    }

    public abstract float GetVisibilityRadius();

    protected abstract void EntityStart();

    public EntityType EntityType { get { return m_entityType; } }
    public int EntityID { get { return m_entityID; } }
    public string EntityName { get { return m_entityName; } }

    protected EntityType m_entityType = EntityType.Unknown;
    protected string m_entityName = "entity_default";

    private int m_entityID = -1;
    private bool m_started = false;
}
