///////////////////////////////////////////////////////////
// 
// AIEditor_menu.cs
//
// What it does: Draws the top-menu portion of the AI-editor graph window.
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
    public void DrawUnzoomedArea()
    {
        AIManager manager = AIManager.s_instance;
        
        string[] taskNames      = AIManager.s_instance.m_taskNames.ToArray();
        string[] actionNames    = AIManager.s_instance.m_actionNames.ToArray();

        EditorGUILayout.BeginVertical(((GUIStyle)("Box")));
        EditorGUILayout.BeginVertical(((GUIStyle)("Box")));
        
        EditorGUILayout.LabelField("Task", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.BeginVertical();
        
        AIManager.s_instance.selectedTaskIndex = EditorGUILayout.Popup(AIManager.s_instance.selectedTaskIndex, taskNames);
        
        if(manager.selectedTaskIndex < manager.m_tasks.Count)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(Screen.width - 90));
            manager.m_tasks[manager.selectedTaskIndex].Name = EditorGUILayout.TextField(manager.m_tasks[manager.selectedTaskIndex].Name);
            manager.m_tasks[manager.selectedTaskIndex].name = manager.m_tasks[manager.selectedTaskIndex].Name;
            manager.m_taskNames[manager.selectedTaskIndex] = manager.m_tasks[manager.selectedTaskIndex].name;
            
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical();
        
        if(GUILayout.Button("New"))
        {
            AIManager.s_instance.AddNewTask();
            AIManager.s_instance.selectedTaskIndex = AIManager.s_instance.m_tasks.Count - 1;
        }
        
        if (GUILayout.Button("Save all"))
        {
            AIManager.s_instance.DoSerialise();
        }
        
        if (GUILayout.Button("Load all"))
        {
            AIManager.s_instance.ReloadTasks();
        }
        
        if (GUILayout.Button("Delete"))
        {
            AIManager.s_instance.DeleteCurrentTask();
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical(((GUIStyle)("Box")));
        
        EditorGUILayout.LabelField("Action", EditorStyles.boldLabel);
        
        
        EditorGUILayout.BeginHorizontal();
        
        if (manager.m_actionNames.Count == 0)
        {
            GUI.enabled = false;
        }
        
        manager.selectedActionIndex = EditorGUILayout.Popup(manager.selectedActionIndex, actionNames);
        
        if (GUILayout.Button("Add to Task", GUILayout.Width(100)))
        {
            if (manager.selectedTaskIndex != -1)
            {
                AIAction newAction = ScriptableObject.CreateInstance(manager.m_actionTypes[manager.selectedActionIndex]) as AIAction;
				newAction.Init();
                manager.m_tasks[manager.selectedTaskIndex].AddAction(newAction);
            }
        }
        
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(((GUIStyle)("Box")));
        
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
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
        
        var LastRect = GUILayoutUtility.GetLastRect();
        manager.m_buttonBarHeight = LastRect.height;
    }
}
