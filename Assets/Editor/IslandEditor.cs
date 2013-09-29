///////////////////////////////////////////////////////////
// 
// IslandEditor.cs
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
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof(Island))]
public class IslandEditor : Editor 
{
	void OnSceneGUI()
	{
		Island island = (Island)target;
		
		if(island.painting)
		{
			if(Event.current.type == EventType.mouseDown && Event.current.button == 0)
			{
				island.mouseDown = true;
			}
			
			if(Event.current.type == EventType.mouseUp && Event.current.button == 0)
			{
				island.mouseDown = false;
			}
			
			Handles.color = Color.green;
			
			float m_sectionSizeX = (island.MaxBounds.x - island.MinBounds.x) / (float)island.SectionsX;
			float m_sectionSizeY = (island.MaxBounds.y - island.MinBounds.y) / (float)island.SectionsY;
			
			float radius = (m_sectionSizeX / 512.0f) * island.brushSize / 2.0f;
			Handles.DrawWireDisc(lastPos, Vector3.up, radius);
			
			Handles.color = Color.blue;
			Handles.DrawLine(Vector3.zero, island.lastWorldPos);
			
			Handles.color = Color.red;
			Handles.DrawLine(island.lastWorldPos, island.lastLocalPos);
			
			if(Event.current.type == EventType.mouseMove)
			{
				Vector2 mousePos = Event.current.mousePosition;
				mousePos.y = Camera.current.pixelHeight - mousePos.y;
				Ray ray = Camera.current.ScreenPointToRay(mousePos);
				
				Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
				
				float rayDistance;
				
				groundPlane.Raycast(ray, out rayDistance);
				
				Vector3 pos = ray.GetPoint(rayDistance);
				
				pos.y = 2;
				
				lastPos = pos;
			}
			
			if(Event.current.type == EventType.mouseDrag && island.mouseDown)
			{
				Vector2 mousePos = Event.current.mousePosition;
				mousePos.y = Camera.current.pixelHeight - mousePos.y;
				Ray ray = Camera.current.ScreenPointToRay(mousePos);
				
				Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
				
				float rayDistance;
				
				groundPlane.Raycast(ray, out rayDistance);
				
				Vector3 pos = ray.GetPoint(rayDistance);
				
				pos.y = 2;
				
				lastPos = pos;
				island.PaintPixel(pos.x, pos.z);
				island.ApplyPaintChanges();
				
			}
			
			if (Event.current.type == EventType.layout)
			{
    			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
				Event.current.Use();
			}
			
			EditorUtility.SetDirty(island);
			
		}
	}
	
	Vector3 lastPos = Vector3.one;
	
	public override void OnInspectorGUI()
	{
		Island island = (Island)target;
		
		if(island.painting)
		{
			if(GUILayout.Button("Stop Painting"))
			{
				island.painting = !island.painting;	
			}
			
			IslandBrushEditor.ShowInspectorGUI(island);
			
			ShowPaintAllInspectorGUI();
			
			GUI.enabled = island.saveRequired;
			if(GUILayout.Button("Save Changes")) 
			{
				island.SaveTextures();	
			}
			GUI.enabled = true;
			
		}
		else
		{ 
			if(GUILayout.Button("Paint"))
			{
				island.painting = !island.painting;	
				
				if(island.painting)
				{
					island.StartPainting();	
				}
			}
			
			island.IslandBaseMaterial = EditorGUILayout.ObjectField(island.IslandBaseMaterial, typeof(Material), true) as Material;
			island.IslandSourceMesh = EditorGUILayout.ObjectField(island.IslandSourceMesh, typeof(Mesh), true) as Mesh;
			
			island.SectionsX = EditorGUILayout.IntField("Slices X", island.SectionsX);
			island.SectionsY = EditorGUILayout.IntField("Slices Y", island.SectionsY);
			
			if(GUILayout.Button("Slice Mesh"))
			{
				if(island.IslandSourceMesh != null)
				{
					EditorUtility.DisplayProgressBar("Slicing Terrain", "Slicing...",  0.0f);	
					
					Mesh[,] meshes = MeshSlice.Slice(island.IslandSourceMesh, island.SectionsX, island.SectionsY, true, true);
						
					GameObject meshesObject = GameObjectHelper.FindChild(island.gameObject, "meshes", true);
					
					island.Sections = new GameObject[meshes.GetLength(0) * meshes.GetLength(1)];
					
					island.MinBounds = new Vector2(island.IslandSourceMesh.bounds.min.x, island.IslandSourceMesh.bounds.min.z); 
					island.MaxBounds = new Vector2(island.IslandSourceMesh.bounds.max.x, island.IslandSourceMesh.bounds.max.z);
					
					AssetDatabase.StartAssetEditing();
					
					for(int x = 0; x < meshes.GetLength(0); x++)
					{
						for(int y = 0; y < meshes.GetLength(1); y++)
						{
							
							EditorUtility.DisplayProgressBar("Slicing Terrain", "Generating Meshes and Textures...",  (float)(x * meshes.GetLength(1) + y) / (float)(meshes.GetLength(0) * meshes.GetLength(1)));
							
							Mesh mesh = meshes[x, y];
							mesh.Optimize();
							
							GameObject newObject = new GameObject("blah");
							
							if(mesh.triangles.Length > 0)
							{
								GameObjectUtility.SetStaticEditorFlags(newObject, StaticEditorFlags.NavigationStatic);	
							}
							
							Debug.Log("Mesh has " + mesh.triangles.Length + " count");
							
							newObject.transform.parent = meshesObject.transform;
							newObject.transform.localPosition = Vector3.zero;
							MeshRenderer renderer = newObject.AddComponent<MeshRenderer>();
							Material newMaterial = new Material(island.IslandBaseMaterial);
							
							
							string path = "Assets/Resources/Textures/Terrain Maps/splat_" + x + "_" + y + ".png";
							
							TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
							
							// TODO: What a load of rotten crow-cock.
							// This is required to make the textures editable for island-painting.
							// Sort out not having fucking great un-compressed textures when running the game.
							if(importer != null)
							{
								TextureImporterSettings settings = new TextureImporterSettings();
								importer.wrapMode = TextureWrapMode.Clamp;
								importer.textureType = TextureImporterType.Advanced;
								importer.ReadTextureSettings(settings);
								importer.textureFormat = TextureImporterFormat.ARGB32;
								importer.isReadable = true;
								importer.compressionQuality = 0;
								importer.textureFormat = TextureImporterFormat.ARGB32;
								settings.readable = true;
								settings.textureFormat = TextureImporterFormat.ARGB32;
								settings.compressionQuality  =0;
								importer.SetTextureSettings(settings);
								
								AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport );	
							}
							
							Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
							
							if(tex == null)
							{
								Debug.LogError("Failed to locate splat-map " + x + "_" + y);	
							}
							
							tex.GetPixels32();
							newMaterial.SetTexture("_Splat", tex);
							
							renderer.sharedMaterial = newMaterial;
							
							MeshFilter filter = newObject.AddComponent<MeshFilter>();
							filter.mesh = mesh;
							
							island.Sections[y * island.SectionsX + x] = newObject;
						}
					}
					
					AssetDatabase.StopAssetEditing();
				}
				
				EditorUtility.ClearProgressBar();
			}
			
			if(GUILayout.Button("Create initial splat-maps"))
			{
				ClearSplatMap();
			}
		}
	}
	
	private void ShowPaintAllInspectorGUI()
	{
		Island island = (Island)target;
		
		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 5));
		
		island.HeightThreshold = EditorGUILayout.FloatField("Height Threshold", island.HeightThreshold);
		island.HeightBlend = EditorGUILayout.FloatField("Height Blend", island.HeightBlend);
		
		if(GUILayout.Button("Paint All"))
		{
			PaintSand();
		}
	}
	
	[MenuItem ("Respite/Island/Clear Splat-Map")]
	public static void ClearSplatMap()
	{
		
		//EditorUtility.dis
		Island island = GameObject.FindObjectOfType(typeof(Island)) as Island;
		
		Color[] colors = new Color[TexWidth*TexHeight];
				
		for(int i = 0; i < TexWidth*TexHeight; i++)
		{
			colors[i] = new Color(1.0f, 0.0f, 0.0f, 0.0f);
		}
		
		for(int x = 0; x < island.SectionsX; x++)
		{
			for(int y = 0; y < island.SectionsY; y++)
			{
				string outputPath = Application.dataPath + "/Resources/Textures/Terrain Maps/splat_" + x + "_" + y + ".png";
		
				string directory = System.IO.Path.GetDirectoryName(outputPath);
				
				if(!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);	
				}
				
				Texture2D newTex = new Texture2D(TexWidth, TexHeight);
				
				newTex.SetPixels(colors);
				
				
				System.IO.File.WriteAllBytes(outputPath, newTex.EncodeToPNG());
			}
		}
		
		island.SaveTextures();
	}
	
	[MenuItem ("Respite/Island/Paint Sand")]
	public static void PaintSand()
	{
		Island island = GameObject.FindObjectOfType(typeof(Island)) as Island; 
		
		int res = 20;
		
		Vector2 islandSize 	= island.MaxBounds - island.MinBounds;
		Vector2 sectionSize = new Vector2(islandSize.x / island.SectionsX, islandSize.y / island.SectionsY);
		Vector2 texelSize 	= new Vector2(sectionSize.x / (TexWidth / res), sectionSize.y / (TexHeight / res));
		
		Debug.Log(texelSize.x);
		
		texelSize = texelSize / 4.0f;
		island.brushSize = res;
		//island.editDetail = false;
		//island.solidBrush = true;
		island.UpdateBrush();
		//island.brushOpacity = 1.0f;
		
		island.StartPainting();
		
		RaycastHit hitInfo;
		
		float max = float.MinValue;
		float min = float.MaxValue;
		
		int counter = 0;
		for(float x = island.MinBounds.x - 10.0f; x < island.MaxBounds.x; x += texelSize.x)
		{
			if(counter % 10 == 0)
			if(EditorUtility.DisplayCancelableProgressBar("Painting sand", "Painting...", (x - island.MinBounds.x) / islandSize.x))
			{
				EditorUtility.ClearProgressBar();
				return;	
			}
			
			
			for(float y = island.MinBounds.y - 10.0f; y < island.MaxBounds.y; y += texelSize.y)
			{
				if(Physics.Raycast(new Vector3(x, 1.0f, y), new Vector3(0.0f, -1.0f, 0.0f), out hitInfo, 50.0f, ~LayerMask.NameToLayer("WorldCollision")))
				{
					if(hitInfo.point.y < island.HeightThreshold)
					{
						if(hitInfo.point.y > island.HeightBlend)
						{
							float opacity = Mathf.Abs((hitInfo.point.y - island.HeightBlend) / (island.HeightThreshold - island.HeightBlend));
							opacity = Mathf.Clamp(opacity, 0.0f, 1.0f);
							//island.paintColor = new Color(0.0f, 0.0f, opacity, 1.0f);
							island.brushOpacity = 1.0f - opacity;
							island.UpdateBrush();
							
							min = Mathf.Min(opacity, min);
							max = Mathf.Max(opacity, max);
						}
						else
						{
							island.brushOpacity = 1.0f;
							island.UpdateBrush();
						}
						
						island.PaintPixel(x, y);
					}
				}
				
				
			}
			counter++;
		}
		
		Debug.Log("Min: " + min);
		Debug.Log("Max: " + max);
		
		EditorUtility.ClearProgressBar();
		island.ApplyPaintChanges();
		
	}
	
	public static int TexWidth = 512;
	public static int TexHeight = 512;
}

public class IslandBrushEditor
{
	public static void ShowInspectorGUI(Island island)
	{
		bool updateBrush = false;
			
			Color paintColor 	= island.paintColor;
			int brushSize 		= island.brushSize;
			float brushOpacity 	= island.brushOpacity;
			bool solidBrush		= island.solidBrush;
			bool editDetail		= island.editDetail;
			
			Color newColor = paintColor;
			
			 
			
			
			if(editDetail)
			{
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
			
			int newSize = EditorGUILayout.IntSlider("Brush Size", island.brushSize, 1, 200);
			float newOpacity = EditorGUILayout.Slider("Brush Opacity", island.brushOpacity, 0.01f, 1.0f);
			bool newSolidBrush = EditorGUILayout.Toggle("Solid Brush", island.solidBrush);
			bool newEditDetail = EditorGUILayout.Toggle("Edit Detail", island.editDetail);
			
			if(newColor != paintColor || newSize != brushSize || newOpacity != brushOpacity || newSolidBrush != solidBrush || newEditDetail != editDetail)
			{
				island.paintColor = newColor;
				island.brushSize = newSize;
				island.brushOpacity = newOpacity;
				island.solidBrush = newSolidBrush;
				island.editDetail = newEditDetail;
				
				island.UpdateBrush();
				
				EditorUtility.SetDirty(island);
			}
		
		if(newColor.r + newColor.g + newColor.b > 1.0f)
			{
				GUILayout.Label("Combined values greater than 1.0f!");	
			}
	}
}

