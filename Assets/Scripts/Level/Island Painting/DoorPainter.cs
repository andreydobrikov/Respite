///////////////////////////////////////////////////////////
// 
// DoorPainter.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class DoorPainter : Painter 
{
	public override void Paint(Island island)
	{
#if UNITY_EDITOR
		IslandBrush brush 		= new IslandBrush();
		IslandBrush detailBrush = new IslandBrush();
		
		brush.m_detailBrush = false;
		brush.m_solidBrush = false;
		brush.m_color = new Color(0.6f, 0.2f, 0.0f, 1.0f);
		brush.m_opacity = 1.0f;
		
		detailBrush.m_detailBrush = true;
		detailBrush.m_solidBrush = false;
		detailBrush.m_color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		detailBrush.m_opacity = 1.0f;
		
		
		
			brush.m_brushSizeX = (int)island.WorldSizeToTexel(transform.localScale.x) * 2;
			brush.Update();
			
			detailBrush.m_brushSizeX = 40;//(int)island.WorldSizeToTexel(transform.localScale.x);
			detailBrush.Update();
			
			island.PaintPixel(transform.position.x, transform.position.z, brush);
			island.PaintPixel(transform.position.x, transform.position.z, detailBrush);
			//island.PaintPixel(m_instancePositions[i].x, m_instancePositions[i].z, detailBrush);
			//island.PaintPixel(m_instancePositions[i].x, m_instancePositions[i].z, detailBrush);
#endif
	}
	
	public override string GetName()
	{
		return "Door (" + name + ")";	
	}
}
