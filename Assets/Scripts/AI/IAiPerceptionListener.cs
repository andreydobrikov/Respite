///////////////////////////////////////////////////////////
// 
// IAiPerceptionListener.cs
//
// What it does: Interface for tracking changes to entity perception.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

public interface IAiPerceptionListener
{
    void EntityEnteredPerception(Entity addedEntity);
    void EntityLeftPerception(Entity removedEntity);
}
