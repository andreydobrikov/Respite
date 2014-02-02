///////////////////////////////////////////////////////////
// 
// ShapePainterEditor.cs
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
using System.Collections.Generic;

[CustomEditor(typeof(ShapePainter))]
public class ShapePainterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ShapePainter painter = (ShapePainter)target;  
        
        painter.m_brush.ShowInspectorGUI();
        
        GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 5));
        
     //   painter.m_heightThreshold   = EditorGUILayout.FloatField("Height Threshold", painter.m_heightThreshold);
     //   painter.m_heightBlend       = EditorGUILayout.FloatField("Height Blend", painter.m_heightBlend);
        
    }

    public void OnSceneGUI()
    {
        Island island = GameObject.FindObjectOfType<Island>();
        ShapePainter painter = (ShapePainter)target; 
        float radiusX = island.GetBrushRadius(painter.m_brush.m_brushSizeX);
        float radiusY = island.GetBrushRadius(painter.m_brush.m_brushSizeY);

        switch(painter.m_brush.m_shape)
        {
            case IslandBrush.IslandBrushShape.Circle:
            {
                Handles.DrawWireDisc(painter.transform.position, Vector3.up, radiusX);
                break;
            }

            case IslandBrush.IslandBrushShape.Square:
            {
                Vector3[] points = new Vector3[5];

                points[0] = painter.transform.position + new Vector3(-radiusX, 0.0f, -radiusY);
                points[1] = painter.transform.position + new Vector3(-radiusX, 0.0f, radiusY);
                points[2] = painter.transform.position + new Vector3(radiusX, 0.0f, radiusY);
                points[3] = painter.transform.position + new Vector3(radiusX, 0.0f, -radiusY);
                points[4] = painter.transform.position + new Vector3(-radiusX, 0.0f, -radiusY);

               

                Handles.DrawPolyLine(points);
                break;
            }
        }

    }
}
