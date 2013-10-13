///////////////////////////////////////////////////////////
// 
// AIEditor.cs
//
// What it does: 
//
// Notes: GUI.Matrix fucks everything up, so this is full of manual offsets
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AIEditor :  EditorWindow 
{
    [MenuItem ("Respite/AI Editor")]
    static void ShowWindow () 
	{
        EditorWindow.GetWindow(typeof(AIEditor));
    }
	
    void OnGUI () 
	{
		// Tells the EditorWindow to listen for MouseMove events
		wantsMouseMove = true;
		
		if(Event.current.type == EventType.mouseDrag)
		{
			if(Event.current.button == 2)
			{
				m_offset += Event.current.mousePosition - m_scrollStart;
				m_scrollStart = Event.current.mousePosition;
			}
		}
		
		// Repaint when the mouse is moved to get smooth animation
		if(Event.current.type == EventType.mouseMove)
		{
			Repaint();
		}
		
		// Only run if an object is selected
		if(Selection.activeGameObject == null)
		{
			return;	
		}
		
		AI activeAI = Selection.activeGameObject.GetComponent(typeof(AI)) as AI;
		
		// Only run if the object has AI
		if(activeAI == null)
		{
			return;	
		}
		
		// Scrolling
		if(Event.current.type == EventType.mouseDown && Event.current.button == 2)
		{
			m_scrollStart = Event.current.mousePosition;
		}
		
		// Button for adding a new state
		if(GUILayout.Button("Add State"))
		{
			AIState newState = ScriptableObject.CreateInstance<AIState>();
			
			newState.Name = "New State " + activeAI.States.Count;
			newState.Parent = activeAI;
			
			activeAI.States.Add(newState);	
		}
		
		// Button for flushing all states
		if(GUILayout.Button("Clear"))
		{
			activeAI.States.Clear(); 
		}
		
		// Draw the temporary line when the user is making links
		if(activeAI.m_dragStart != null)
		{
			AIBehaviour source = activeAI.m_dragStart;
			
			Vector2 start = new Vector2(source.m_parentState.m_editorPosition.x + source.m_lastBounds.width + m_offset.x, source.m_parentState.m_editorPosition.y + m_offset.y);
			Vector2 end = Event.current.mousePosition;
			
			Drawing.DrawLine(start, end, Color.red, 1.0f, true);	
			
			Repaint();
		}
		
		// TODO: Loads of these loops can be removed with decent layering
		
		foreach(var state in activeAI.States)
		{
			foreach(var behaviour in state.Behaviours)
			{
				if(behaviour.TransitionTarget != null)
				{
					Vector2 start = new Vector2(state.m_editorPosition.x + state.m_editorPosition.width + stateHandleSize.x / 2.0f, state.m_editorPosition.y + behaviour.m_lastBounds.y + stateHandleSize.y / 2.0f);
					Vector2 end = new Vector2(behaviour.TransitionTarget.m_editorPosition.x - stateHandleSize.x / 2.0f, behaviour.TransitionTarget.m_editorPosition.y + behaviour.TransitionTarget.m_editorPosition.height / 2.0f + stateHandleSize.y / 2.0f);
					
					start.x += m_offset.x;
					start.y += m_offset.y;
					
					end.x += m_offset.x;
					end.y += m_offset.y;
					
					Drawing.bezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.yellow, 1.0f, true, 20);
				}
			}
		}
		
		
		foreach(var state in activeAI.States)
		{
			if(GUI.Button(new Rect(state.m_editorPosition.x - 10 + m_offset.x , state.m_editorPosition.y + state.m_editorPosition.height / 2.0f + m_offset.y, 10, 10), "x"))
			{
				if(activeAI.m_dragStart != null && !state.Behaviours.Contains(activeAI.m_dragStart))
				{
					activeAI.m_dragStart.TransitionTarget = state;
					activeAI.m_dragStart = null;
				}
			}
		}
		
		
		
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			if(activeAI.m_dragStart != null)
			{
				activeAI.m_dragStart.TransitionTarget = null;
			}
			
			activeAI.m_dragStart = null;	
		}
		
		foreach(var state in activeAI.States)
		{
			foreach(var behaviour in state.Behaviours)
			{
				if(behaviour.SupportsTransitions)
				{
					if(GUI.Button(new Rect(state.m_editorPosition.x + state.m_editorPosition.width + m_offset.x, state.m_editorPosition.y + behaviour.m_lastBounds.y + m_offset.y, 10, 10), "x"))
					{
						behaviour.TransitionTarget = null;
						activeAI.m_dragStart = behaviour;
					}
				}
			}
		}   
		
		BeginWindows();
		
		for(int i = 0; i < activeAI.States.Count; i++)
		{	
			Rect position = activeAI.States[i].m_editorPosition;
			position.x += m_offset.x;
			position.y += m_offset.y;
			
			activeAI.States[i].m_editorPosition = GUILayout.Window(i, position, DrawWindow1, activeAI.States[i].Name + (i == activeAI.StartStateIndex ? " (Default)" : ""));
			
			activeAI.States[i].m_editorPosition.x -= m_offset.x;
			activeAI.States[i].m_editorPosition.y -= m_offset.y;
		}
		
		EndWindows();
		
		if(Event.current.type == EventType.mouseDown )
		{
			activeAI.SelectedState = -1;
		}
	}
	
	public void OnInspectorUpdate()
	{
		Repaint();
	}
			
	void DrawWindow1(int id)
	{
		m_toDelete.Clear();
		
		AI activeAI = Selection.activeGameObject.GetComponent(typeof(AI)) as AI;
		
		if(Event.current.type == EventType.mouseDown)
		{
			activeAI.SelectedState = id;
		}
		
		AIState currentState = activeAI.States[id];
		
		currentState.m_showFoldout = EditorGUILayout.Foldout(currentState.m_showFoldout, "State Settings"); 
		
		if(currentState.m_showFoldout)
		{
			GUILayout.BeginHorizontal();
			
			currentState.Name = EditorGUILayout.TextField(currentState.Name);
			
			if(GUILayout.Button("X", GUILayout.Width(30)))
			{
				if(EditorUtility.DisplayDialog("Delete State?", "This will delete state \"" + currentState.Name + "\"", "delete", "cancel"))
				{
					activeAI.DeleteState(currentState);
					return;	
				}
			}
			
			GUILayout.EndHorizontal();
			
			if(activeAI.StartStateIndex != id)
			{
				if(GUILayout.Toggle(false, "Default State"))
				{
					activeAI.StartStateIndex = id;	
				}
			}
		}
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		foreach(var behaviour in currentState.Behaviours)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			behaviour.m_showFoldout = EditorGUILayout.Foldout(behaviour.m_showFoldout, behaviour.Name);
			
			if(Event.current.type == EventType.Repaint)
			{
				behaviour.m_lastBounds = GUILayoutUtility.GetLastRect();
			}
						
			if(behaviour.m_showFoldout)
			{
				behaviour.OnInspectorGUI();
				
				if(GUILayout.Button("Delete"))
				{
					m_toDelete.Add(behaviour);	
				}
			}
			
			GUILayout.EndVertical();
		}
		
		GUILayout.EndVertical();
		
		foreach(var behaviour in m_toDelete)
		{
			currentState.Behaviours.Remove(behaviour);	
		}
		
		if(AIManager.s_instance != null)
		{
		
			EditorGUILayout.BeginHorizontal();
			
			currentState.m_behaviourToAdd = EditorGUILayout.Popup(currentState.m_behaviourToAdd, AIManager.s_instance.AvailableBehaviourNames.ToArray());	
			
			if(GUILayout.Button("Add"))
			{
				AIBehaviour newBehaviour = ScriptableObject.CreateInstance(AIManager.s_instance.AvailableBehaviours[currentState.m_behaviourToAdd]) as AIBehaviour;
				newBehaviour.m_parentState = currentState;
			
				currentState.Behaviours.Add(newBehaviour);
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		if(currentState.Running)
		{
			GUILayout.Label("State Running");	
		}
		
		GUI.DragWindow();	
		
		
	}
	
	private static List<AIBehaviour> m_behaviours = new List<AIBehaviour>();
	private static List<AIBehaviour> m_toDelete = new List<AIBehaviour>();
	private static Vector2 m_offset = Vector2.zero;
	private static Vector2 m_scrollStart = Vector2.zero;
	private static float lineCurveScale = 70.0f;
	private static Vector2 stateHandleSize = new Vector2(10.0f, 10.0f);
}
