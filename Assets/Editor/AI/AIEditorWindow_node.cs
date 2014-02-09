///////////////////////////////////////////////////////////
// 
// AIEditorWindow_node.cs
//
// What it does: Draws a given AIAction node in the AI-editor graph.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class AIEditorWindow :  EditorWindow 
{
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
}
