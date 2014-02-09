///////////////////////////////////////////////////////////
// 
// AIEditor_graph.cs
//
// What it does: Draws the graph section of the AI-editor graph window.
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
    private static Vector2 m_scrollOffset   = Vector2.zero; // Offset for drag-scroll.
    private static Vector2 m_scrollStart    = Vector2.zero; // Mouse-down point for drag-scroll.
    private static float lineCurveScale     = 70.0f;        // Offset of the link line from its origin (adjusts points 1,2 of spline points 0,1,2,3)
    
    private float m_lastWidth   = 200.0f;
    private float m_lastHeight  = 200.0f;

    public void DrawZoomArea()
    {
        AIManager manager = AIManager.s_instance;

        // Start zoomed drawing using the dimensions of the menu section as bounds.
        EditorZoomArea.Begin(manager.m_zoom, new Rect(0.0f, manager.m_buttonBarHeight, position.width, position.height));

        // Mouse-down scrolling
        if (Event.current.type == EventType.mouseDown && Event.current.button == 2)
        {
            m_scrollStart = Event.current.mousePosition;
        }
    
        // These checks fire when I've bodged data during development
        if (manager.selectedTaskIndex == -1)                    { return; }
        if (manager.selectedTaskIndex >= manager.m_tasks.Count) { return; }


        AITask currentTask = manager.m_tasks[manager.selectedTaskIndex];
        
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
                        
                        Drawing.DrawLine(start, right, Color.red, 1.0f, true);
                        Drawing.DrawLine(right, rightTop, Color.red, 1.0f, true);
                        Drawing.DrawLine(rightTop, leftTop, Color.red, 1.0f, true);
                        Drawing.DrawLine(leftTop, left, Color.red, 1.0f, true);
                        Drawing.DrawLine(left, end, Color.red, 1.0f, true);
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
            //  activeAI.SelectedState = -1;
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

    

}
