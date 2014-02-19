///////////////////////////////////////////////////////////
// 
// ShapePainter.cs
//
// What it does: Paints a given shape onto the island splat-map.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ShapePainter : Painter 
{


    public void OnEnable()
    {
        if(m_brush == null)
        {
            m_brush = new IslandBrush();
        }
    }

    public enum ShapePainterType
    {
        Circle,
        Square
    }

    public override void Paint(Island island)
	{
#if UNITY_EDITOR
		island.PaintPixel(transform.position.x, transform.position.z, m_brush);
#endif
	}

    public override string GetName()
    {
        return "ShapePainter (" + gameObject.name + ")";
    }

    public IslandBrush m_brush;

}
