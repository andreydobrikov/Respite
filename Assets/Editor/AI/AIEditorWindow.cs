///////////////////////////////////////////////////////////
// 
// AIEditorWindow.cs
//
// What it does: 
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
    [MenuItem ("Respite/AI Editor")]
    static void ShowWindow () 
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
   
    // Force the graph to redraw if the inspector is updated.
    public void OnInspectorUpdate()
    {
        Repaint();
    }
}
