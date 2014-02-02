///////////////////////////////////////////////////////////
// 
// HeightPainter.cs
//
// What it does: Paints the entire splat-map according to height-values.
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class HeightPainter : Painter 
{
	public override void Paint(Island island)
	{
#if UNITY_EDITOR
		
		int res = 20;
		
		Vector2 islandSize 	= island.MaxBounds - island.MinBounds;
		Vector2 sectionSize = new Vector2(islandSize.x / island.SectionsX, islandSize.y / island.SectionsY);
		Vector2 texelSize 	= new Vector2(sectionSize.x / (Island.TexWidth / res), sectionSize.y / (Island.TexHeight / res));
		
		texelSize = texelSize / 4.0f;
		 
		m_brush.m_brushSizeX = res;
		m_brush.Update();
		
		RaycastHit hitInfo;
		
		float max = float.MinValue;
		float min = float.MaxValue;
		
		int counter = 0;
		for(float x = island.MinBounds.x - 10.0f; x < island.MaxBounds.x; x += texelSize.x)
		{
			if(counter % 10 == 0)
			{
				if(EditorUtility.DisplayCancelableProgressBar("Painting sand", "Painting...", (x - island.MinBounds.x) / islandSize.x))
				{
					EditorUtility.ClearProgressBar();
					return;	
				}
			}
			
			for(float y = island.MinBounds.y - 10.0f; y < island.MaxBounds.y; y += texelSize.y)
			{
				if(Physics.Raycast(new Vector3(x, 1.0f, y), new Vector3(0.0f, -1.0f, 0.0f), out hitInfo, 50.0f, ~LayerMask.NameToLayer("WorldCollision")))
				{
					if(hitInfo.point.y < m_heightThreshold)
					{
						if(hitInfo.point.y > m_heightBlend)
						{
							float opacity = Mathf.Abs((hitInfo.point.y - m_heightBlend) / (m_heightThreshold - m_heightBlend));
							opacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
							
							m_brush.m_opacity = 1.0f - opacity;
							m_brush.Update();
							
							min = Mathf.Min(opacity, min);
							max = Mathf.Max(opacity, max);
						}
						else
						{
							m_brush.m_opacity = 1.0f;
							m_brush.Update();
						}
						
						island.PaintPixel(x, y, m_brush);
					}
				}
			}
			counter++;
		}
		
		EditorUtility.ClearProgressBar();
#endif
	}
	
	public override string GetName()
	{
		return "Height Painter (" + gameObject.name + ")";
	}
	
	public IslandBrush m_brush 		= new IslandBrush();
	public float m_heightThreshold 	= 0.0f;
	public float m_heightBlend		= 0.0f;
}
