///////////////////////////////////////////////////////////
// 
// IslandBrush.cs
//
// What it does: Why, golly. It defines a brush to be used when painting the Island's splat-map.
//
// Notes: Also provides a stock function for editing brushes in the inspector GUI.
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IslandBrush
{
	public bool m_solidBrush 		= false;			// Whether the brush fades at the edges
	public Color m_color	 		= Color.white;
	public int m_brushSize 			= 20;
	public float m_opacity 	= 1.0f;
	public bool m_detailBrush 		= false;
	
	public Color[] m_brushPixels; 						// The actual color array the brush represents.
	
	public void Update()
	{
		m_brushPixels = new Color[m_brushSize * m_brushSize];
		
		for(int x = 0; x < m_brushSize; x++)
		{
			for(int y = 0; y < m_brushSize; y++)
			{
				Color currentColor = m_color;
				
				// Loads of this can go outside the loop. Also, reciprocals once
				float relativeX = Mathf.Abs(((float)m_brushSize / 2.0f) - (float)x) / ((float)m_brushSize / 2.0f);
				float relativeY = Mathf.Abs(((float)m_brushSize / 2.0f) - (float)y) / ((float)m_brushSize / 2.0f);
				
				float val = 1.0f - Mathf.Sqrt(relativeX * relativeX + relativeY * relativeY);
				
				// TODO: This is bullshit
				if(m_solidBrush && val > 0.2f)
				{
					val = 1.0f;	
				}
				
				currentColor.a = (val * m_opacity);
				
				m_brushPixels[y * m_brushSize + x] = currentColor;
			}	
		}
	}
	
#if UNITY_EDITOR
	public void ShowInspectorGUI()
	{
		// The island is required to display the splat-maps materials in the inspector. Bummer.
		Island island = GameObject.FindObjectOfType(typeof(Island)) as Island;
		
		Color paintColor 	= m_color;
		int brushSize 		= m_brushSize;
		float brushOpacity 	= m_opacity;
		bool solidBrush		= m_solidBrush;
		bool editDetail		= m_detailBrush;
		
		Color newColor = paintColor;
		
		if(editDetail)
		{
			// When editing detail, the detail intensity maps to the Red channel of the paint texture.
			newColor.r = EditorGUILayout.Slider("Detail Intensity", paintColor.r, 0.0f, 1.0f);	
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.Box(island.texture0, GUILayout.Width(64), GUILayout.Height(64));
			newColor.r = EditorGUILayout.Slider(newColor.r, 0.0f, 1.0f);
			GUILayout.EndHorizontal();
			
			GUILayout.Box("", GUILayout.Width(Screen.width - 10), GUILayout.Height(1));
			
			GUILayout.BeginHorizontal();
			GUILayout.Box(island.texture1, GUILayout.Width(64), GUILayout.Height(64));
			newColor.g = EditorGUILayout.Slider(newColor.g, 0.0f, 1.0f);
			GUILayout.EndHorizontal();
			
			GUILayout.Box("", GUILayout.Width(Screen.width - 10), GUILayout.Height(1));
			
			GUILayout.BeginHorizontal();
			GUILayout.Box(island.texture2, GUILayout.Width(64), GUILayout.Height(64));
			newColor.b = EditorGUILayout.Slider(newColor.b, 0.0f, 1.0f);
			GUILayout.EndHorizontal();
			
			GUILayout.Box("", GUILayout.Width(Screen.width - 10), GUILayout.Height(1));
		}
		
		int newSize 		= EditorGUILayout.IntSlider("Brush Size", m_brushSize, 1, 200);
		float newOpacity 	= EditorGUILayout.Slider("Brush Opacity", m_opacity, 0.01f, 1.0f);
		bool newSolidBrush 	= EditorGUILayout.Toggle("Solid Brush", m_solidBrush);
		bool newEditDetail 	= EditorGUILayout.Toggle("Edit Detail", m_detailBrush);
		
		if(newColor != paintColor || newSize != brushSize || newOpacity != brushOpacity || newSolidBrush != solidBrush || newEditDetail != editDetail)
		{
			m_color = newColor;
			m_brushSize = newSize;
			m_opacity = newOpacity;
			m_solidBrush = newSolidBrush;
			m_detailBrush = newEditDetail;
			
			Update();
			
			EditorUtility.SetDirty(island);
		}
	
		if(newColor.r + newColor.g + newColor.b > 1.0f)
		{
			GUILayout.Label("Combined values greater than 1.0f!");	
		}
	}
#endif
}