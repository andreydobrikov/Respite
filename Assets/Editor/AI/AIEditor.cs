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
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AI))] 
public class AIEditor : Editor
{
	public override void OnInspectorGUI()
	{
		AI ai = (AI)target;

		System.Type targetType = typeof(AIBehaviour);
		List<System.Type> types = new List<System.Type>(targetType.Assembly.GetTypes().Where(x => x.IsSubclassOf(targetType)));

		List<System.Type> aiTypes = new List<System.Type>();
		foreach(var aiType in ai.Behaviours)
		{
			aiTypes.Add(aiType.GetType());
		}

		types = new List<System.Type>(types.Where(x => !aiTypes.Contains(x)));

		string[] typeStrings = new string[types.Count];

		int index = 0;
		foreach(var currentType in types)
		{
			typeStrings[index] = currentType.Name;
			index++;
		}

		GUILayout.BeginHorizontal();

		if(types.Count == 0)
		{
			GUI.enabled = false;
		}

		ai.m_selectedBehaviourIndex = Mathf.Min(EditorGUILayout.Popup(ai.m_selectedBehaviourIndex, typeStrings), Mathf.Max(0, types.Count - 1));

		if(GUILayout.Button("Add", GUILayout.Width(50)))
		{
			ai.Behaviours.Add(ScriptableObject.CreateInstance(types[ai.m_selectedBehaviourIndex]) as AIBehaviour);
		}

		GUI.enabled = true;

		GUILayout.EndHorizontal();
		List<AIBehaviour> toDelete = new List<AIBehaviour>();

		foreach(var behaviour in ai.Behaviours)
		{
			GUILayout.BeginHorizontal();

			GUILayout.Label(behaviour.Name);

			if(GUILayout.Button("Delete"))
			{
				toDelete.Add(behaviour);
			}

			GUILayout.EndHorizontal();
		}

		foreach(var deletedBehaviour in toDelete)
		{
			ai.Behaviours.Remove(deletedBehaviour);
		}
	}
}

public class AIEditorWindow :  EditorWindow 
{
	[MenuItem ("Respite/AI Editor")]
	static void ShowWindow () 
	{
		EditorWindow.GetWindow(typeof(AIEditorWindow));
	}

	[MenuItem ("Respite/AI Manager Deserialise")]
	static void TestDeserialise () 
	{
		EditorWindow.GetWindow(typeof(AIEditorWindow));
	}
	
	void OnGUI () 
	{
		// Tells the EditorWindow to listen for MouseMove events
		wantsMouseMove = true;

		DrawUnzoomedArea();
		DrawZoomArea();
	}

	public void DrawUnzoomedArea()
	{
		AIManager manager = AIManager.s_instance;

		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
	
		string[] taskNames = AIManager.s_instance.m_taskNames.ToArray();
		string[] actionNames = AIManager.s_instance.m_actionNames.ToArray();

		AIManager.s_instance.selectedTaskIndex = EditorGUILayout.Popup(AIManager.s_instance.selectedTaskIndex, taskNames);
		
		if (GUILayout.Button("Save all tasks", GUILayout.Width(150)))
		{
			AIManager.s_instance.DoSerialise();
		}

		if (GUILayout.Button("Reload all tasks", GUILayout.Width((150))))
		{
			AIManager.s_instance.ReloadTasks();
		}

		if (GUILayout.Button("Reset View", GUILayout.Width(80)))
		{
			if (manager.selectedTaskIndex != -1 && manager.m_tasks[manager.selectedTaskIndex].Actions.Count > 0)
			{
				var action = manager.m_tasks[manager.selectedTaskIndex].Actions[0];
				m_scrollOffset = new Vector2(action.m_editorPosition.x, action.m_editorPosition.y);
			}
			else
			{
				m_scrollOffset = Vector2.zero;
			}
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		if (manager.m_actionNames.Count == 0)
		{
			GUI.enabled = false;
		}

		manager.selectedActionIndex = EditorGUILayout.Popup(manager.selectedActionIndex, actionNames);

		if (GUILayout.Button("Add Action"))
		{
			if (manager.selectedTaskIndex != -1)
			{
				AIAction newAction = ScriptableObject.CreateInstance(manager.m_actionTypes[manager.selectedActionIndex]) as AIAction;
				manager.m_tasks[manager.selectedTaskIndex].AddAction(newAction);
			}
		}

		GUI.enabled = true;

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		var LastRect = GUILayoutUtility.GetLastRect();
		manager.m_buttonBarHeight = LastRect.height;
	}

	public void DrawZoomArea()
	{
		AIManager manager = AIManager.s_instance;

		EditorZoomArea.Begin(manager.m_zoom, new Rect(0.0f, manager.m_buttonBarHeight, position.width, position.height));


		// Scrolling
		if (Event.current.type == EventType.mouseDown && Event.current.button == 2)
		{
			m_scrollStart = Event.current.mousePosition;
		}

		GUILayout.BeginHorizontal();

		if (manager.selectedTaskIndex == -1)
		{
			return;
		}

		// This can happen during twiddling
		if (manager.selectedTaskIndex >= manager.m_tasks.Count)
		{
				return;
		}

		AITask currentTask = manager.m_tasks[manager.selectedTaskIndex];



		// Button for adding a new action
		/*
		if(GUILayout.Button("Add Action", GUILayout.Width(80)))
		{
			AIState newState = ScriptableObject.CreateInstance<AIState>();
			
			newState.Name = "New State " + activeAI.States.Count;
			newState.Parent = activeAI;

			newState.m_editorPosition.x = Screen.width / 2;
			newState.m_editorPosition.y = Screen.height / 2;
			
			activeAI.States.Add(newState);	
		}*/




		GUILayout.EndHorizontal();
		//return;
		// Draw the temporary line when the user is making links
		/*
		if(manager.m_dragStart != null)
		{
			AIAction source = manager.m_dragStart;

			Vector2 start = new Vector2(source.m_editorPosition.x + source.m_windowWidth + stateHandleSize.x / 2.0f + m_scrollOffset.x, source.m_editorPosition.y + source.m_lastBounds.y + m_scrollOffset.y + stateHandleSize.y / 2.0f);
			Vector2 end = Event.current.mousePosition;

			Drawing.bezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.red, 1.0f, true, 20);
		}
		*/
		// TODO: Loads of these loops can be removed with decent layering
		// TODO: Only redraw when moved or connections changed!
		// Draw connections

		Color red = Color.red;
		red.a = 0.4f;
		foreach (var action in currentTask.Actions)
		{
			float delta = 1.0f / action.Outputs.Count;
			float currentVal = 0.0f;
			float expansion = 30.0f;
			for (int index = 0; index < action.Outputs.Count; index++)
			{
				AIAction linkedAction = action.GetOutput(index);
				if (linkedAction != null)
				{
					Vector2 start = new Vector2(action.m_editorPosition.x + action.Outputs[index].outputRect.x + action.Outputs[index].outputRect.width + 10, action.m_editorPosition.y + action.Outputs[index].outputRect.y + 10);
					Vector2 end = new Vector2(linkedAction.m_editorPosition.x - 5, linkedAction.m_editorPosition.y + linkedAction.m_lastBounds.height / 2.0f);

					start.x += m_scrollOffset.x;
					start.y += m_scrollOffset.y;

					end.x += m_scrollOffset.x;
					end.y += m_scrollOffset.y;

					if (action == linkedAction)
					{
						Vector2 right = start + (new Vector2(10.0f + (expansion * currentVal), 0.0f));
						Vector2 rightTop = right;
						rightTop.y = action.m_editorPosition.y + m_scrollOffset.y - 50 - (expansion * currentVal);

						Vector2 leftTop = rightTop;
						leftTop.x = end.x - 10.0f - (expansion * currentVal);

						Vector2 left = leftTop;
						left.y = end.y;

						Drawing.DrawLine(start, right, red, 1.0f, true);
						Drawing.DrawLine(right, rightTop, red, 1.0f, true);
						Drawing.DrawLine(rightTop, leftTop, red, 1.0f, true);
						Drawing.DrawLine(leftTop, left, red, 1.0f, true);
						Drawing.DrawLine(left, end, red, 1.0f, true);
					}
					else
					{
						Drawing.DrawBezierLine(start, start + (Vector2.right * lineCurveScale), end, end + (new Vector2(-1.0f, 0.0f) * lineCurveScale), Color.yellow, 1.0f, true, 20);
					}
				}
				currentVal += delta;
			}
		}

		// input button checks
		foreach (var action in currentTask.Actions)
		{
			if (GUI.Button(new Rect(action.m_editorPosition.x - 10 + m_scrollOffset.x, action.m_editorPosition.y + action.m_lastBounds.height / 2.0f + m_scrollOffset.y - 5, 10, 10), "x"))
			{
				if (manager.m_dragAction != null)
				{
					manager.m_dragAction.SetOutput(manager.m_dragAction.Outputs[manager.m_dragActionOutput].linkName, action);
				}
			}
		}
		//Debug.Log(currentTask.m_actiontest);
		// Clear drag events on left-mouse click
		if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			manager.m_dragAction = null;
		}

		// Show behaviour link output buttons
		foreach (var action in currentTask.Actions)
		{
			int index = 0;
			foreach (var output in action.Outputs)
			{
				GUI.depth = -1;
				if (GUI.Button(new Rect(action.m_editorPosition.x + action.Outputs[index].outputRect.x + action.Outputs[index].outputRect.width + m_scrollOffset.x + 5, action.m_editorPosition.y + action.Outputs[index].outputRect.y + m_scrollOffset.y + 5, 10, 10), "x"))
				{

					action.SetOutput(output.linkName, null);
					manager.m_dragAction = action;
					manager.m_dragActionOutput = index;
				}
				index++;
			}

		}

		GUI.depth = 1;
		BeginWindows();

		for (int i = 0; i < currentTask.Actions.Count; i++)
		{
			Rect currentPos = currentTask.Actions[i].m_editorPosition;
			currentPos.x += m_scrollOffset.x;
			currentPos.y += m_scrollOffset.y;

			string name = currentTask.Actions[i].Name;

			Rect thing = GUILayout.Window(i, currentPos, DrawActionWindow, name);

			currentTask.Actions[i].m_editorPosition = thing;

			currentTask.Actions[i].m_editorPosition.x -= m_scrollOffset.x;
			currentTask.Actions[i].m_editorPosition.y -= m_scrollOffset.y;

			if (Event.current.type == EventType.repaint)
			{
				currentTask.Actions[i].m_windowWidth = thing.width;
				currentTask.Actions[i].m_lastBounds = thing;
			}
		}

		EndWindows();

		if (Event.current.type == EventType.mouseDown)
		{
			//	activeAI.SelectedState = -1;
		}

		// I have no idea why, but this nukes drag-scrolling if it's near the top of the function
		if (Event.current.type == EventType.mouseDrag)
		{
			if (Event.current.button == 2)
			{
				m_scrollOffset += (Event.current.mousePosition - m_scrollStart) / 2.0f;
				m_scrollStart = Event.current.mousePosition;
			}
		}

		if (Event.current.type == EventType.ScrollWheel)
		{
			manager.m_zoom -= ((float)Event.current.delta.y * 0.01f);

			manager.m_zoom = Mathf.Max(0.5f, manager.m_zoom);
			manager.m_zoom = Mathf.Min(1.0f, manager.m_zoom);
		}

		// This is pretty shonky, but keeps things smooth.
		// Disable if performance gets choppy and work out something better
		Repaint();

		EditorZoomArea.End();
	}
	
	public void OnInspectorUpdate()
	{
		Repaint();
	}
			
	// Draw an editor window
	void DrawActionWindow(int id)
	{
		AIAction currentAction = AIManager.s_instance.m_tasks[AIManager.s_instance.selectedTaskIndex].Actions[id];
		GUIStyle rightStyle = new GUIStyle((GUIStyle)("label"));
		rightStyle.alignment = TextAnchor.MiddleRight;

		GUIStyle leftStyle = new GUIStyle((GUIStyle)("label"));
		leftStyle.alignment = TextAnchor.MiddleLeft;

		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("test", leftStyle);

		GUILayout.BeginVertical();

		int index = 0;
		foreach (var output in currentAction.Outputs)
		{
			GUILayout.Label(output.linkName, rightStyle);
			if (Event.current.type == EventType.repaint)
			{
				var link = currentAction.Outputs[index];
				link.outputRect = GUILayoutUtility.GetLastRect();
				currentAction.Outputs[index] = link;
			}
			
			index++;
		}

		currentAction.m_showInputDataFoldout = EditorGUILayout.Foldout(currentAction.m_showInputDataFoldout, "Input Data");

		if(currentAction.m_showInputDataFoldout)
		{
			for(int i = 0; i < currentAction.m_inputData.Count; i++)
			{
				var actionData = currentAction.m_inputData[i];
				GUILayout.BeginHorizontal();

				GUILayout.Label(actionData.DataID, leftStyle);
				actionData.BlackboardSourceID = GUILayout.TextField(actionData.BlackboardSourceID != null ? actionData.BlackboardSourceID : "", GUILayout.Width(100));

				currentAction.m_inputData[i] = actionData;

				GUILayout.EndHorizontal();
			}
		}


		GUILayout.EndVertical();

		m_lastWidth = currentAction.m_editorPosition.width;

		if (Event.current.type == EventType.repaint)
		{
			m_lastHeight = 0.0f;
			m_lastWidth = currentAction.Outputs.Count > 0 ? 200.0f : currentAction.m_editorPosition.width;
		}

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		/*

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

					GUILayout.BeginHorizontal();

					behaviour.m_showFoldout = EditorGUILayout.Foldout(behaviour.m_showFoldout, behaviour.Name);
					behaviour.Enabled = EditorGUILayout.Toggle(behaviour.Enabled, GUILayout.Width(30));

					GUILayout.EndHorizontal();

					GUI.enabled = behaviour.Enabled;

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

					GUI.enabled = true;

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

	
		*/

		if (Event.current.type == EventType.repaint)
		{
			currentAction.m_editorPosition.height = m_lastHeight;
			currentAction.m_editorPosition.width = m_lastWidth;

			currentAction.m_renderDimensions.x = m_lastWidth;
			currentAction.m_renderDimensions.y = m_lastHeight;
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
