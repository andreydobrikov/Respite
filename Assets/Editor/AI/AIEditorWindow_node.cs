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

        if(GUILayout.Button("Delete"))
        {
            AIManager.s_instance.m_tasks[AIManager.s_instance.selectedTaskIndex].DeleteAction(currentAction);
        }

        GUILayout.EndVertical();
   
        
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
