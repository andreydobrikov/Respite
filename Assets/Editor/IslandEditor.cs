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
			/*
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
			*/
			
		}
	}
	
	public override void OnInspectorGUI()
	{
		Island island = (Island)target;
		
		Painter[] painters = GameObject.FindObjectsOfType(typeof(Painter)) as Painter[];
		
		System.Array.Sort(painters);
		
		int count = 0;
		foreach(var painter in painters)
		{
			GUI.enabled = true;
			
			GUILayout.BeginHorizontal();
			
			painter.ShouldPaint = GUILayout.Toggle(painter.ShouldPaint, "", GUILayout.Width(20), GUILayout.Height(15));
			
			GUI.enabled = painter.ShouldPaint;
			
			GUILayout.Label(painter.GetName());
			
			if(count > 0)
			{
				if(GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(15)))
				{
					int index = painter.Index;
					painter.Index = painters[count - 1].Index;
					painters[count - 1].Index = index; 
				}
			}
			
			if(count < painters.Length - 1) 
			{
				if(GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
				{
					int index = painter.Index;
					painter.Index = painters[count + 1].Index;
					painters[count + 1].Index = index; 
				}
				
			}
			
			GUILayout.EndHorizontal();
			count++;
		}
		
		System.Array.Sort(painters);
		
		count = 0;
		foreach(var painter in painters) 
		{
			painter.Index = count;	
			count++;
		}
		
		GUI.enabled = true;
		if(GUILayout.Button("Repaint"))
		{
			AssetDatabase.StartAssetEditing();
			
			island.StartPainting();
			
			ClearSplatMap();
			
			foreach(var painter in painters)
			{
				if(painter.ShouldPaint)
				{
					painter.Paint(island);
				}
			}
			
			island.ApplyPaintChanges();
			island.SaveTextures();
			
			for(int x = 0; x < island.SectionsX; x++)
			{
				for(int y = 0; y < island.SectionsY; y++)
				{
					string path = "Assets/Textures/Terrain Maps/splat_" + x + "_" + y + ".png";
					
					AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport );
				}
			}
			
			AssetDatabase.StopAssetEditing();
			
			EditorUtility.ClearProgressBar();
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
							
							GameObject newObject = new GameObject("island_section_" + x + "_" + y);
							
						//	if(mesh.triangles.Length > 0)
							{
							//	GameObjectUtility.SetStaticEditorFlags(newObject, StaticEditorFlags.NavigationStatic);	
							}
							
							Debug.Log("Mesh has " + mesh.triangles.Length + " count");
							
							newObject.transform.parent = meshesObject.transform;
							newObject.transform.localPosition = Vector3.zero;
							MeshRenderer renderer = newObject.AddComponent<MeshRenderer>();
							Material newMaterial = new Material(island.IslandBaseMaterial);
							
							
							string path = "Assets/Textures/Terrain Maps/splat_" + x + "_" + y + ".png";
							
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
		
		return;
	}
    	
	[MenuItem ("Respite/Misc/Flush Progress Bar")]
	public static void FlushProgressBar()
	{
		EditorUtility.ClearProgressBar();	
	}
	
	[MenuItem ("Respite/Island/Clear Splat-Map")]
	public static void ClearSplatMap()
	{
		Island island = GameObject.FindObjectOfType(typeof(Island)) as Island;
		
		island.ClearSplatMap();
		
	}
}


