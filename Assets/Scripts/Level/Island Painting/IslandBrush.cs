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
    public enum IslandBrushShape
    {
        Circle,
        Square
    }

    public IslandBrushShape m_shape     = IslandBrushShape.Circle;  
	public bool m_solidBrush 		    = false;			        // Whether the brush fades at the edges
	public Color m_color	 		    = Color.white;
	public int m_brushSizeX 			= 20;
    public int m_brushSizeY             = 20;
	public float m_opacity 	            = 1.0f;
	public bool m_detailBrush 		    = false;
    public float m_fadeOutPercentage    = 0.2f;
	
	public Color[] m_brushPixels; 						// The actual color array the brush represents.
	
    // This function sets the actual pixels of the brush for use when painting
    // Call infrequently for many good success
	public void Update()
	{
		
		
        switch(m_shape)
        {
            case IslandBrushShape.Circle:
            {
                m_brushPixels = new Color[m_brushSizeX * m_brushSizeX];
                for(int x = 0; x < m_brushSizeX; x++)
                {
                    for(int y = 0; y < m_brushSizeX; y++)
                    {
                        Color currentColor = m_color;
                        
                        // Loads of this can go outside the loop. Also, reciprocals once
                        float relativeX = Mathf.Abs(((float)m_brushSizeX / 2.0f) - (float)x) / ((float)m_brushSizeX / 2.0f);
                        float relativeY = Mathf.Abs(((float)m_brushSizeX / 2.0f) - (float)y) / ((float)m_brushSizeX / 2.0f);
                        
                        float val = 1.0f - Mathf.Sqrt(relativeX * relativeX + relativeY * relativeY);
                        val = (val) / (m_fadeOutPercentage * 2.0f);
                        val = Mathf.Clamp01(val);
                        
                        // TODO: This is bullshit
                        if(m_solidBrush && val > m_fadeOutPercentage)
                        {
                            val = 1.0f; 
                        }
                        
                        currentColor.a = (val * m_opacity);
                        
                        m_brushPixels[y * m_brushSizeX + x] = currentColor;
                    }   
                }
                break;
            }

            case IslandBrushShape.Square:
            {
                const float fadeThreshold = 0.2f;

                m_brushPixels = new Color[m_brushSizeX * m_brushSizeY];

                for(int x = 0; x < m_brushSizeX; x++)
                {
                    for(int y = 0; y < m_brushSizeY; y++)
                    {
                        float xDiff = Mathf.Abs(Mathf.Min(x, ((float)m_brushSizeX - x))) / (float)m_brushSizeX;
                        float yDiff = Mathf.Abs(Mathf.Min(y, ((float)m_brushSizeY - y))) / (float)m_brushSizeY;

                        xDiff = Mathf.Clamp01(xDiff / m_fadeOutPercentage);
                        yDiff = Mathf.Clamp01(yDiff / m_fadeOutPercentage);

                        float xAlpha = Mathf.Lerp(0.0f, m_opacity, xDiff);
                        float yAlpha = Mathf.Lerp(0.0f, m_opacity, yDiff);

                        Color currentColor = m_color;
                        currentColor.a = (xAlpha * yAlpha);
                      //  Debug.Log("New alpha: " + xDiff / (m_brushSize / fadeThreshold));

                        if(m_solidBrush)
                        {
                            currentColor.a = 1.0f;
                        }

                        m_brushPixels[y * m_brushSizeX + x] = currentColor;
                    }
                }
                break;
            }
        }
		
	}
	
#if UNITY_EDITOR
	public void ShowInspectorGUI()
	{
		// The island is required to display the splat-maps materials in the inspector. Bummer.
		Island island = GameObject.FindObjectOfType(typeof(Island)) as Island;
		
		Color paintColor 	                = m_color;
		int brushSizeX		                = m_brushSizeX;
        int brushSizeY                      = m_brushSizeY;
		float brushOpacity 	                = m_opacity;
		bool solidBrush		                = m_solidBrush;
		bool editDetail		                = m_detailBrush;
        IslandBrushShape shape              = m_shape;
        float fadePercentage                = m_fadeOutPercentage;
		
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
		
        IslandBrushShape newShape               = (IslandBrushShape)EditorGUILayout.EnumPopup(shape);

        int newSizeX                            = m_brushSizeX;
        int newSizeY                            = m_brushSizeY;

        if(newShape == IslandBrushShape.Circle)
        {
            newSizeX                            = EditorGUILayout.IntSlider("Brush Size", m_brushSizeX, 1, 400);
        }
        else
        {
            newSizeX                            = EditorGUILayout.IntSlider("Brush Size X", m_brushSizeX, 1, 400);
            newSizeY                            = EditorGUILayout.IntSlider("Brush Size Y", m_brushSizeY, 1, 400);
        }
		
		float newOpacity 	                    = EditorGUILayout.Slider("Brush Opacity", m_opacity, 0.01f, 1.0f);
		bool newSolidBrush 	                    = EditorGUILayout.Toggle("Solid Brush", m_solidBrush);
		bool newEditDetail 	                    = EditorGUILayout.Toggle("Edit Detail", m_detailBrush);
        float newFadePercentage                  = fadePercentage;
        if(!m_solidBrush)
        {
            newFadePercentage                   = EditorGUILayout.Slider("Fade Percentage", m_fadeOutPercentage, 0.0f, 0.5f);
        }
		
		if(newColor != paintColor || 
           newSizeX != brushSizeX || 
           newSizeY != brushSizeY || 
           newOpacity != brushOpacity || 
           newSolidBrush != solidBrush || 
           newEditDetail != editDetail || 
           newShape != shape ||
           newFadePercentage != fadePercentage)
		{
			m_color = newColor;
			m_brushSizeX = newSizeX;
            m_brushSizeY = newSizeY;
			m_opacity = newOpacity;
			m_solidBrush = newSolidBrush;
			m_detailBrush = newEditDetail;
            m_shape = newShape;
            m_fadeOutPercentage = newFadePercentage;
			
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