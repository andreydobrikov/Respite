#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Path : Painter
{
	public Path()
	{
		m_initialised = false;
		m_spline = new Spline();
	}
	
	void OnEnable()
	{
		if(!m_initialised)
		{
			m_spline.Start();
			// The splines will default to being at the origin, so shift them to the position of the path.
			m_spline.Offset(transform.position);
			
			m_initialised = true;
		}
	}
	
	public override void Paint(Island island)
	{
#if UNITY_EDITOR
		m_detailBrush.m_color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		m_detailBrush.m_solidBrush = false;
		m_detailBrush.m_opacity = 1.0f;
		m_detailBrush.m_detailBrush = true;
		
		int blobCount = (int)(m_spline.GetLength() / 0.01f);
		
		float delta = 1.0f / (float)blobCount;
		for(int i = 0; i < blobCount; i++)
		{
			if(i % 10 == 0)
			{
				EditorUtility.DisplayProgressBar("Painting blobs", "Paint blob: " + i + "\\" + blobCount, (float)i / (float)blobCount);
			}
			
			m_brush.m_brushSize = (int)island.WorldSizeToTexel(m_spline.GetWidth(delta * i) * (m_meshWidth * 2.0f));
			m_brush.Update();
			
			m_detailBrush.m_brushSize = m_brush.m_brushSize;
			m_detailBrush.Update();
			
			Vector2 position = m_spline.GetPosition(delta * i);
			
			island.PaintPixel(position.x, position.y, m_brush);
			island.PaintPixel(position.x, position.y, m_detailBrush);
		}
		EditorUtility.ClearProgressBar();
#endif
	}
	
	public override string GetName()
	{
		return "Path (" + gameObject.name + ")";
	}
	
#if UNITY_EDITOR
	
	public float m_uv0Multiplier			= 1.0f;
	public float m_uv1Multiplier			= 1.0f;
	public float m_meshWidth				= 0.2f;
	public float m_meshDepth				= -1.0f;
	public Material m_meshMaterial			= null;
	public int m_meshSegmentCount			= 30;
	public int m_meshLayer					= 0;
	
	public int m_colliderSectionCount 		= 1;
	public float m_colliderWidthMultiplier 	= 1.0f;
	
	public bool m_showCollidersFoldout		= false;
	public bool m_showMeshesFoldout			= false;
	public bool m_showGeneratorFoldout 		= false;
	public bool m_showPaintFoldout			= false;
	public bool m_colliders					= true;
	public bool m_meshes					= true;
	public bool m_paint						= true;
	public bool m_drawPathOnly 				= false;
	
	public IslandBrush m_brush				= new IslandBrush();
	public IslandBrush m_detailBrush		= new IslandBrush();
	
#endif
	
	
	public Spline m_spline;
	
	[SerializeField]
	private bool m_initialised;
}
