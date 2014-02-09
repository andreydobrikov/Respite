///////////////////////////////////////////////////////////
// 
// EntityManager.cs
//
// What it does: 
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class EntityManager
{
    public static EntityManager Instance
    {
        get
        {
            if(s_instance == null)
            {
                s_instance = new EntityManager();
            }

            return s_instance;
        }
    }

    public int RegisterEntity(Entity entity)
    {
        Debug.Log("Added entity: " + entity.EntityName);

        m_entities.Add(entity);
        return m_maxEntityID++;
    }

    private static EntityManager s_instance = null;
    private List<Entity> m_entities = new List<Entity>();

    private int m_maxEntityID = 0;
}
