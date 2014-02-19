///////////////////////////////////////////////////////////
// 
// AI.cs
//
// What it does: The basic Monobehaviour controlling a given NPC's AI
//
// Notes: 	The AI has a number of behaviours that control the entity's actions.
//			These behaviours can issue tasks to be performed. Each task is in turn comprised of individual actions,
//			each of which is an atomic operation such as navigation, interaction with an object, animation, etc.
//		 
//			AI is currently the most complex element of the game to serialise, so shit.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(SphereCollider))]
public class AI : MonoBehaviour, ISerialisable
{
    // Public Methods
	public void Start ()
	{
        m_perceptionRangeCollider = GetComponent<SphereCollider>();

		m_blackboard = new AIBlackboard();

		foreach(var behaviour in m_behaviours)
		{
			behaviour.LoadTasks();
			behaviour.m_parentAI = this;
			behaviour.RegisterBlackboardEntries();
			behaviour.Start();
		}

		m_blackboard.Initialised = true;
	}

	public void Update ()
	{
		foreach(var behaviour in m_behaviours)
		{
			behaviour.Update();
		}

		m_taskList.Sort();

		// If any tasks are queued, ensure the highest priority is running
		if(m_taskList.Count > 0)
		{
			if(m_runningTask != m_taskList[0])
			{
				if(m_runningTask != null)
				{
					m_runningTask.Suspend(AITaskSuspendPriority.Normal);
				}

				m_runningTask = m_taskList[0];
				m_runningTask.Start();

#if AI_LOGGING
				Debug.Log("Starting AI task \"" + m_runningTask.name + "\"");
#endif
			}
		}

		// Update the current task
		if(m_runningTask != null)
		{
			m_runningTask.Update();

			// If the task is complete, chuck it out.
			if(m_runningTask.Result == AITaskResult.Complete)
			{
				m_taskList.Remove(m_runningTask);
				m_runningTask.Reset();
				m_runningTask = null;
			}
		}
	}

	// TODO: Save the AI!
	public void SaveSerialise(List<SavePair> pairs){}

	// TODO: Load the AI!
	public void SaveDeserialise(List<SavePair> pairs){}

	public void PushTask(AITask task)
	{
		task.Result = AITaskResult.Pending;
		m_taskList.Add(task);
	}

    public void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null)
        {
            m_entitiesInPerception.Add(entity);

            foreach(var listener in m_perceptionListeners)
            {
                listener.EntityEnteredPerception(entity);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if(entity != null)
        {
            m_entitiesInPerception.Remove(entity);

            foreach(var listener in m_perceptionListeners)
            {
                listener.EntityLeftPerception(entity);
            }
        }
    }

    public void RegisterPerceptionListener(IAiPerceptionListener listener)
    {
        m_perceptionListeners.Add(listener);
    }

    public void UnregisterPerceptionListener(IAiPerceptionListener listener)
    {
        m_perceptionListeners.Remove(listener);
    }

	public void OnGUI()
	{
#if UNITY_EDITOR
		foreach(var behaviour in m_behaviours)
		{
			behaviour.OnSceneGUI();
		}
#endif
	}

    // Properties
    public AIBlackboard Blackboard      { get { return m_blackboard; } }
    public List<AIBehaviour> Behaviours { get { return m_behaviours; } }
    public List<Entity> PerceivedEntitites { get { return m_entitiesInPerception; } }

    // Private Fields
	[SerializeField]
	private List<AIBehaviour> m_behaviours                      = new List<AIBehaviour>();          // Behaviours used by this AI
	private AIBlackboard m_blackboard 	                        = null;					            // AI blackboard containing all AI data
	private AITask m_runningTask 		                        = null;					            // The currently running AI task
	private List<AITask> m_taskList 	                        = new List<AITask>();	            // All tasks queued
    private SphereCollider m_perceptionRangeCollider            = null;                             // The collider to detect when an entity moves into the AI's perception-range
    private List<Entity> m_entitiesInPerception                 = new List<Entity>();
    private List<IAiPerceptionListener> m_perceptionListeners   = new List<IAiPerceptionListener>();
	
#if UNITY_EDITOR
	public int m_selectedBehaviourIndex = 0;    
	public AIBehaviour m_dragStart      = null;
	public bool m_debugEditor           = true;
#endif
}
