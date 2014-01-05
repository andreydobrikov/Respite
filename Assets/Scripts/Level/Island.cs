///////////////////////////////////////////////////////////
// 
// Island.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Island : MonoBehaviour 
{
	public int SectionsX = 10;
	public int SectionsY = 10;
	
	public Vector2 MinBounds = Vector2.zero;
	public Vector2 MaxBounds = Vector2.zero;
	
	public GameObject[] Sections;
	
	public Material IslandBaseMaterial = null;
	public Mesh IslandSourceMesh = null;
	
	void OnEnable()
	{
#if UNITY_EDITOR
		painting = false;	
#endif
	}
	
	void Start()
	{
		m_sectionSize = (MaxBounds.x - MinBounds.x) / (float)SectionsX;
	}

	public void StartPainting()
	{
#if UNITY_EDITOR
		m_editTextures = new ColorBlock[SectionsX * SectionsY];
		
		for(int x = 0; x < SectionsX; x++)
		{
			for(int y = 0; y < SectionsY; y++)
			{	

				EditorUtility.DisplayProgressBar("Starting Painting", "Grabbing Texture Data " + x + ", " + y, (y * SectionsX + x) / (SectionsX * SectionsY));		

				
				m_editTextures[y * SectionsX + x] = new ColorBlock();
				
				Texture2D tex = Sections[y * SectionsX + x].renderer.sharedMaterial.GetTexture("_Splat") as Texture2D;
				
				m_editTextures[y * SectionsX + x].splatTex = tex;
				m_editTextures[y * SectionsX + x].colors = tex.GetPixels();
			}
		}
		
		if(Sections.Length >= 1)
		{
			texture0 = Sections[0].renderer.sharedMaterial.GetTexture("_Layer0") as Texture2D;
			texture1 = Sections[0].renderer.sharedMaterial.GetTexture("_Layer1") as Texture2D;
			texture2 = Sections[0].renderer.sharedMaterial.GetTexture("_Layer2") as Texture2D;
		}
		
		EditorUtility.ClearProgressBar();	
#endif
		
	}
	
#if UNITY_EDITOR
	 
	/// plonk the brush down at the given point.
	/// More complicated than I would have liked thanks to the sliced terrain.
	/// TODO: Sort the importing so that read/write is disabled on the splat maps when building
	/// outside the editor. Re-enable compression so the splat-maps aren't a meg each.
	public void PaintPixel(float x, float y, IslandBrush brush)
	{
		m_sectionSize = (MaxBounds.x - MinBounds.x) / (float)SectionsX;
		
		if(x > MinBounds.x && y > MinBounds.y && x < MaxBounds.x && y < MaxBounds.y)
		{
			SectionPixelPair pair = GetPair(x, y);

					
			int xStart = pair.pixelX - brush.m_brushSize / 2;
			int xEnd = pair.pixelX + brush.m_brushSize / 2;
					
			int yStart = pair.pixelY - brush.m_brushSize / 2;
			int yEnd = pair.pixelY + brush.m_brushSize / 2;
			
			for(int currentX = xStart; currentX < xEnd; currentX++)
			{
				for(int currentY = yStart; currentY < yEnd; currentY++)
				{
					int sectionX = pair.sectionX;
					int sectionY = pair.sectionY;
					int pixelX = currentX;
					int pixelY = currentY; 
					
					while(pixelX < 0)
					{
						sectionX--;
						pixelX += 512;
					}
					
					while(pixelX >= 512)
					{
						sectionX++;
						pixelX -= 512;
					}
					
					while(pixelY < 0)
					{
						sectionY--;
						pixelY += 512;
					}
					
					while(pixelY >= 512)
					{
						sectionY++;
						pixelY -= 512;
					}
					
					if(sectionX >= SectionsX || sectionY >= SectionsY || sectionX < 0 || sectionY < 0)
					{
						continue;	
					}
					
					int index = (currentY - yStart) * brush.m_brushSize + (currentX - xStart);
					
					Color current = m_editTextures[sectionY * SectionsX + sectionX].colors[pixelY * 512 + pixelX];
					
					
					if(brush.m_detailBrush)
					{
						// TODO: What a complete hack.
						// The brush's alpha channel is being used to store the brush dynamics, so when trying to write alpha I just
						// bung the target value into R. How shameful.
						float newColor = brush.m_brushPixels[index].r;
						m_editTextures[sectionY * SectionsX + sectionX].colors[pixelY * 512 + pixelX].a = Mathf.Lerp(current.a, newColor,  brush.m_brushPixels[index].a);	
					}
					else
					{
						float alpha = current.a; // Don't lerp alpha in this scenario, else you'll bugger t'detail
						Vector3 newColor = (Vector3)((Vector4)(brush.m_brushPixels[index]));
						m_editTextures[sectionY * SectionsX + sectionX].colors[pixelY * 512 + pixelX] = Vector4.Lerp(current, newColor,  brush.m_brushPixels[index].a);	
						m_editTextures[sectionY * SectionsX + sectionX].colors[pixelY * 512 + pixelX].a = alpha;
					}
					
					m_editTextures[sectionY * SectionsX + sectionX].dirty = true;
					m_editTextures[sectionY * SectionsX + sectionX].saveRequired = true;
					
					saveRequired = true;
				}
			}
			
			
		}
	}
	
	public void ApplyPaintChanges()
	{
		for(int i = 0; i < m_editTextures.Length; i++)
		{
			if(m_editTextures[i].dirty)
			{
				m_editTextures[i].splatTex.SetPixels(m_editTextures[i].colors);
				m_editTextures[i].splatTex.Apply();		
				m_editTextures[i].dirty = false;
			}
		}
	}
	
	public void ClearSplatMap()
	{
		string test;
		Color[] clearColors = new Color[TexWidth * TexHeight];
		for(int i = 0; i < TexWidth * TexHeight; i++)
		{
			clearColors[i] = new Color(0.8f, 0.2f, 0.0f, 0.3f);	
		}
		
		for(int i = 0; i < m_editTextures.Length; i++)
		{
			Array.Copy(clearColors, m_editTextures[i].colors, clearColors.Length);
			m_editTextures[i].dirty = true;
			m_editTextures[i].saveRequired = true;
			saveRequired = true;
		}
	}
	
	public void SaveTextures()
	{
		int saveCount = 0;
		
		for(int i = 0; i < m_editTextures.Length; i++)
		{
			EditorUtility.DisplayProgressBar("Saving Splat-Map Textures", "Saving...", (float)i / (float)m_editTextures.Length);
			
			if(m_editTextures[i].saveRequired)
			{
				string assetPath = AssetDatabase.GetAssetPath(m_editTextures[i].splatTex);	
				System.IO.File.WriteAllBytes(assetPath, m_editTextures[i].splatTex.EncodeToPNG());	
				m_editTextures[i].saveRequired = false;
				saveCount++;
			}
		}
		
		EditorUtility.ClearProgressBar();
		
		Debug.Log("Saved " + saveCount + " sections");
		saveRequired = false;
	}
	
	// TODO: These are rather broken as they assume the world is square
	public float WorldSizeToTexel(float worldSize)
	{
		m_sectionSize = (MaxBounds.x - MinBounds.x) / (float)SectionsX;
		
		float texelSize = (worldSize / m_sectionSize) * 512.0f;
		
		return texelSize;
	}
	
	private SectionPixelPair GetPair(float x, float y)
	{
		// 0-1
		float relativeX = (x - MinBounds.x	) / (MaxBounds.x - MinBounds.x);
		float relativeY = (y - MinBounds.y	) / (MaxBounds.y - MinBounds.y);
		
		// 0 - section count
		int sectionX = (int)(relativeX * (float)SectionsX);
		int sectionY = (int)(relativeY * (float)SectionsY);
		
		float localX = relativeX - (sectionX / (float)SectionsX);
		float localY = relativeY - (sectionY / (float)SectionsY);
		
		int pixelX = (int)(localX * SectionsX * 512.0f);
		int pixelY = (int)(localY * SectionsY * 512.0f);
		
		SectionPixelPair pair;
		pair.sectionX = sectionX;
		pair.sectionY = sectionY;
		pair.pixelX = pixelX;
		pair.pixelY = pixelY;
		
		return pair;
	}
	
#endif
	
	public static int TexWidth = 512;
	public static int TexHeight = 512;
	
	private float m_sectionSize;
	
#if UNITY_EDITOR
	public bool painting = false;
	public bool mouseDown = false;
	
	public int m_width;
	public int m_height;
	
	[SerializeField]
	public bool saveRequired 	= false;
	
	public List<MeshSlice.Triangle> m_triangles = new List<MeshSlice.Triangle>();
	
	public Vector2 sliceStart = Vector2.zero;
	public Vector2 sliceEnd = Vector2.one;
	
	public Vector3 lastWorldPos = Vector3.zero;
	public Vector3 lastLocalPos = Vector3.zero;
	
	public Texture2D texture0;
	public Texture2D texture1;
	public Texture2D texture2;
	

	
	[SerializeField]
	public ColorBlock[] m_editTextures;
	
	[SerializeField]
	public IslandBrush m_currentBrush;
	
	[Serializable]
	public struct ColorBlock
	{
		[SerializeField]
		public Color[] colors;
		
		[SerializeField]
		public Texture2D splatTex;
		
		[SerializeField]
		public bool dirty;
		
		[SerializeField]
		public bool saveRequired;
	}
	
	public struct SectionPixelPair
	{
		public int sectionX, sectionY;	
		public int pixelX, pixelY;
	}
	
	
#endif
}
