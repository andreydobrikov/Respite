///////////////////////////////////////////////////////////
// 
// Actor.cs
//
// What it does: An entity that possesses an AI.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class Actor : Entity
{
    public override float GetVisibilityRadius()
    {//TODO: Hook this into something sensible.
        return 3.0f;
    }

    protected override void EntityStart()
    {
        m_entityName = "Actor";
        m_entityType = EntityType.Actor;
    }
}
