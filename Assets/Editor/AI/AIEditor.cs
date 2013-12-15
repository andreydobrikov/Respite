///////////////////////////////////////////////////////////
// 
// AIEditor.cs
//
// What it does: 
//
// Notes: GUI.Matrix fucks everything up, so this is full of manual offsets
// 
// To-do: Clean up those fucking dimension mathematics
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

		GUILayout.BeginHorizontal();

		// Button for adding a new state
		if(GUILayout.Button("Add State", GUILayout.Width(80)))
		{
			AIState newState = ScriptableObject.CreateInstance<AIState>();
			
			newState.Name = "New State " + activeAI.States.Count;
			newState.Parent = activeAI;

			newState.m_editorPosition.x = Screen.width / 2;
			newState.m_editorPosition.y = Screen.height / 2;
			
			activeAI.States.Add(newState);	
		}
		
		// Button for flushing all states
		if(GUILayout.Button("Clear", GUILayout.Width(50)))
		{
			activeAI.States.Clear(); 
		}

		if(GUILayout.Button("Reset View", GUILayout.Width(80)))
		{
			m_scrollOffset = Vector2.zero;
		}

		activeAI.m_debugEditor = GUILayout.Toggle(activeAI.m_debugEditor, "Debug", GUILayout.Width(80));

		GUILayout.EndHorizontal();
		
		// Draw the temporary line when the user is making links
		if(activeAI.m_dragStart != null)
		{
			AIBehaviour source = activeAI.m_dragStart;

			Vector2 start = new Vector2(source.m_parentState.m_editorPosition.x + source.m_parentState.m_windowWidth + stateHandleSize.x / 2.0f + m_scrollOffset.x, source.m_parentState.m_editorPosition.y + source.m_lastBounds.y + m_scrollOffset.y + stateHandleSize.y / 2.0f);
			Vector2 end = Event.current.mousePosition;

			Drawing.bezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.red, 1.0f, true, 20);
		}

		// TODO: Loads of these loops can be removed with decent layering
		// TODO: Only redraw when moved or connections changed!
		// Draw connections
		foreach(var state in activeAI.States)
		{
			foreach(var behaviour in state.Behaviours)
			{
				if(behaviour.TransitionTarget != null)
				{
					if(state.m_showBehavioursFoldout)
					{

						Vector2 start = new Vector2(state.m_editorPosition.x + state.m_windowWidth + stateHandleSize.x / 2.0f, state.m_editorPosition.y + behaviour.m_lastBounds.y + stateHandleSize.y / 2.0f);
						Vector2 end = new Vector2(behaviour.TransitionTarget.m_editorPosition.x - stateHandleSize.x / 2.0f, behaviour.TransitionTarget.m_editorPosition.y + behaviour.TransitionTarget.m_renderDimensions.y / 2.0f + stateHandleSize.y / 2.0f);
						
						start.x += m_scrollOffset.x;
						start.y += m_scrollOffset.y;
						
						end.x += m_scrollOffset.x;
						end.y += m_scrollOffset.y;
						
						Drawing.bezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.yellow, 1.0f, true, 20);
					
					}
					else
					{
						Vector2 start = new Vector2(state.m_editorPosition.x + state.m_editorPosition.width + stateHandleSize.x / 2.0f, state.m_editorPosition.y + state.m_behaviourRenderRect.y + (state.m_behaviourRenderRect.height / 2)  );
						Vector2 end = new Vector2(behaviour.TransitionTarget.m_editorPosition.x - stateHandleSize.x / 2.0f, behaviour.TransitionTarget.m_editorPosition.y + behaviour.TransitionTarget.m_renderDimensions.y / 2.0f + stateHandleSize.y / 2.0f);
						
						start.x += m_scrollOffset.x;
						start.y += m_scrollOffset.y;
						
						end.x += m_scrollOffset.x;
						end.y += m_scrollOffset.y;
						
						Drawing.bezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.yellow, 1.0f, true, 20);


					}
				}
			}
		}
		
		// input button checks
		foreach(var state in activeAI.States)
		{
			if(GUI.Button(new Rect(state.m_editorPosition.x - 10 + m_scrollOffset.x , state.m_editorPosition.y + state.m_renderDimensions.y / 2.0f + m_scrollOffset.y, 10, 10), "x"))
			{
				if(activeAI.m_dragStart != null && !state.Behaviours.Contains(activeAI.m_dragStart))
				{
					activeAI.m_dragStart.TransitionTarget = state;
					activeAI.m_dragStart = null;
				}
			}
		}
		
		// Clear drag events on left-mouse click
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			if(activeAI.m_dragStart != null)
			{
				activeAI.m_dragStart.TransitionTarget = null;
			}
			
			activeAI.m_dragStart = null;	
		}
		
		// Show behaviour link output buttons
		foreach(var state in activeAI.States)
		{
			if (state.m_showBehavioursFoldout)
			{
				foreach (var behaviour in state.Behaviours)
				{
					if (behaviour.SupportsTransitions)
					{
						if (GUI.Button(new Rect(state.m_editorPosition.x + state.m_windowWidth + m_scrollOffset.x, state.m_editorPosition.y + behaviour.m_lastBounds.y + m_scrollOffset.y, 10, 10), "x"))
						{
							behaviour.TransitionTarget = null;
							activeAI.m_dragStart = behaviour;
						}
					}
				}
			}
			else if(state.Behaviours.Count > 0)
			{
				bool supportTransitions = false;

				foreach (var behaviour in state.Behaviours)
				{
					if (behaviour.SupportsTransitions)
					{
						supportTransitions = true;
						break;
					}
				}

				if(supportTransitions)
				{
					GUI.enabled = false;
					GUI.Button(new Rect(state.m_editorPosition.x + state.m_editorPosition.width + m_scrollOffset.x, state.m_editorPosition.y + state.m_behaviourRenderRect.y + (state.m_behaviourRenderRect.height / 2) - (stateHandleSize.y / 2.0f) + m_scrollOffset.y, 10, 10), "x");
					GUI.enabled = true;
				}

			}
		}   

		BeginWindows();
		
		for(int i = 0; i < activeAI.States.Count; i++)
		{	
			Rect position = activeAI.States[i].m_editorPosition;
			position.x += m_scrollOffset.x;
			position.y += m_scrollOffset.y;

			string name = activeAI.States[i].Name;

			if(activeAI.m_debugEditor)
			{
				name += " (" + activeAI.States[i].m_editorPosition.x.ToString("0.00") + ", " + activeAI.States[i].m_editorPosition.y.ToString("0.00") + ")";
			}

			Rect thing = GUILayout.Window(i, position, DrawWindow1, name + (i == activeAI.StartStateIndex ? " (Default)" : ""));

				activeAI.States[i].m_editorPosition = thing;
				
				activeAI.States[i].m_editorPosition.x -= m_scrollOffset.x;
				activeAI.States[i].m_editorPosition.y -= m_scrollOffset.y; 

			if(Event.current.type == EventType.repaint )
			{
				activeAI.States[i].m_windowWidth = thing.width;
			}
		}
		
		EndWindows();
		
		if(Event.current.type == EventType.mouseDown )
		{
			activeAI.SelectedState = -1;
		}

		// I have no idea why, but this nukes drag-scrolling if it's near the top of the function
		if(Event.current.type == EventType.mouseDrag)
		{
			if(Event.current.button == 2)
			{
				m_scrollOffset += Event.current.mousePosition - m_scrollStart;
				m_scrollStart = Event.current.mousePosition;
			}
		}

		// This is pretty shonky, but keeps things smooth.
		// Disable if performance gets choppy and work out something better
		Repaint();
	}
	
	public void OnInspectorUpdate()
	{
		Repaint();
	}
			
	// Draw an editor window
	void DrawWindow1(int id)
	{

		m_toDelete.Clear();
		
		AI activeAI = Selection.activeGameObject.GetComponent(typeof(AI)) as AI;
		
		if(Event.current.type == EventType.mouseDown)
		{
			activeAI.SelectedState = id;
		}
		
		AIState currentState = activeAI.States[id];
		m_lastWidth = currentState.m_editorPosition.width;

		if(Event.current.type == EventType.repaint)
		{
			m_lastHeight = 0.0f;
			m_lastWidth = currentState.Behaviours.Count > 0 ? 200.0f : currentState.m_editorPosition.width;
		}

		GUILayout.BeginVertical((GUIStyle)("Box"));
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

		GUILayout.EndVertical();

		if(Event.current.type == EventType.repaint)
		{
			m_lastHeight += GUILayoutUtility.GetLastRect().height;
		}

		GUILayout.BeginVertical((GUIStyle)("Box"));

		currentState.m_showBehavioursFoldout = EditorGUILayout.Foldout(currentState.m_showBehavioursFoldout, "Behaviours");

		if (currentState.m_showBehavioursFoldout)
		{
			if (currentState.Behaviours.Count > 0)
			{
				GUILayout.BeginVertical((GUIStyle)("Box"));

				foreach (var behaviour in currentState.Behaviours)
				{
					GUILayout.BeginVertical((GUIStyle)("Box"));

					behaviour.m_showFoldout = EditorGUILayout.Foldout(behaviour.m_showFoldout, behaviour.Name);

					if (behaviour.m_showFoldout)
					{
						behaviour.OnInspectorGUI();

						if(Event.current.type == EventType.repaint)
						{
							Rect lastRect = GUILayoutUtility.GetLastRect();
							if (lastRect.width > m_lastWidth) m_lastWidth = lastRect.width;
						}

						if (GUILayout.Button("Delete"))
						{
							m_toDelete.Add(behaviour);
						}
					}

					GUILayout.EndVertical();
					if (Event.current.type == EventType.Repaint)
					{
						behaviour.m_lastBounds = GUILayoutUtility.GetLastRect();
					}

					if(Event.current.type == EventType.repaint)
					{
						Rect lastRect = GUILayoutUtility.GetLastRect();
						if (lastRect.width > m_lastWidth) m_lastWidth = lastRect.width;
					}
				}

				GUILayout.EndVertical();
			}


			foreach (var behaviour in m_toDelete)
			{
				currentState.Behaviours.Remove(behaviour);
			}

			if (AIManager.s_instance != null)
			{

				EditorGUILayout.BeginHorizontal();

				currentState.m_behaviourToAdd = EditorGUILayout.Popup(currentState.m_behaviourToAdd, AIManager.s_instance.AvailableBehaviourNames.ToArray());

				if (GUILayout.Button("Add"))
				{
					AIBehaviour newBehaviour = ScriptableObject.CreateInstance(AIManager.s_instance.AvailableBehaviours[currentState.m_behaviourToAdd]) as AIBehaviour;
					newBehaviour.m_parentState = currentState;

					currentState.Behaviours.Add(newBehaviour);
				}

				EditorGUILayout.EndHorizontal();


			}
		}

		GUILayout.EndVertical();

		if(currentState.Behaviours.Count > 0)
		{
			float width = GUILayoutUtility.GetLastRect().width;
			if(width > m_lastWidth) m_lastWidth = width;
		}

		if(Event.current.type == EventType.repaint)
		{
			currentState.m_behaviourRenderRect = GUILayoutUtility.GetLastRect();

			m_lastHeight += GUILayoutUtility.GetLastRect().height;
		}
		
		if(currentState.Running)
		{
			GUILayout.Label("State Running");	
		}

		if (Event.current.type == EventType.repaint)
		{
			currentState.m_editorPosition.height = m_lastHeight;
			currentState.m_editorPosition.width = m_lastWidth;

			currentState.m_renderDimensions.x = m_lastWidth;
			currentState.m_renderDimensions.y = m_lastHeight;
		}

		GUI.DragWindow();	
	}
	
	private static List<AIBehaviour> m_toDelete = new List<AIBehaviour>();
	private static Vector2 m_scrollOffset = Vector2.zero;
	private static Vector2 m_scrollStart = Vector2.zero;
	private static float lineCurveScale = 70.0f;
	private static Vector2 stateHandleSize = new Vector2(10.0f, 10.0f);

	private float m_lastWidth = 200.0f;
	private float m_lastHeight = 200.0f;
}
