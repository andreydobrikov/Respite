///////////////////////////////////////////////////////////
// 
// BuildingEditor.cs
//
// What it does: Generates game-objects for building components
//
// Notes: Fucking revolting
// 
// To-do: Drop it down a well and inform the village that you put it up for adoption.
//		  Full of repeated code.
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Building))] 
public class BuildingEditor :  Editor 
{
	public override void OnInspectorGUI()
	{
		Building building = (Building)target;
		
		EditorGUILayout.BeginVertical();
		
		building.BuildingName = EditorGUILayout.TextField("Building Name", building.BuildingName);
		
		EditorGUILayout.BeginVertical((GUIStyle)("Box"));
		
		EditorGUILayout.LabelField("Rooms");
		
		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(Screen.width - 15));
		
		List<Room> toDelete = new List<Room>();
		
		for(int roomID = 0; roomID < building.Rooms.Count; ++roomID)
		{
			Room current = building.Rooms[roomID];
			
			EditorGUILayout.BeginHorizontal();
			
			
			current.ShowFoldout = EditorGUILayout.Foldout(current.ShowFoldout, current.Name);
			
			if(GUILayout.Button("Delete"))
			{
				toDelete.Add(current);	
			}
			
			EditorGUILayout.EndHorizontal();
			
			if(current.ShowFoldout)
			{
				EditorGUILayout.BeginVertical((GUIStyle)("Box"));
				
				EditorGUI.indentLevel = 1;
				
				current.Name = EditorGUILayout.TextField(current.Name);
				current.TODMaxColor = EditorGUILayout.ColorField("TOD Max Colour", current.TODMaxColor);
				current.TODMinColor = EditorGUILayout.ColorField("TOD Min Colour", current.TODMinColor);
				
				if(GUILayout.Button("Generate Lightmap"))
				{
					GenerateLightmap(current);
				}
				
				EditorGUI.indentLevel = 0;
				
				EditorGUILayout.EndVertical();
			}
		}
		
		foreach(var room in toDelete)
		{
			building.Rooms.Remove(room);	
		}
		
		
		if(GUILayout.Button("Add Room"))
		{
			building.Rooms.Add(new Room());	
		}
		
		EditorGUILayout.EndVertical(); 
		
		if(GUILayout.Button("Rebuild"))
		{
			RebuildWalls();
			RebuildRooms();
			BuildWeatherMask();
		}
	
		bool cancel = false;
		if(GUILayout.Button("Rebuild All Lightmaps"))
		{
			int currentRoom = 0;
			foreach(var room in building.Rooms)
			{
				cancel = EditorUtility.DisplayCancelableProgressBar("Generating Lightmaps", room.Name,  (float)currentRoom /  (float)building.Rooms.Count);	
				if(cancel)
				{
					break;	
				}
				GenerateLightmap(room);
				currentRoom++;
			}
		}
		
		building.floorHeight = EditorGUILayout.IntField("Floor", building.floorHeight);
		
		if(GUILayout.Button("Set Floor"))
		{
			Vector3 pos = building.transform.position;
			pos.y = building.floorHeight * 5.0f;
			building.transform.position = pos;
		}
		
		EditorUtility.ClearProgressBar();
		EditorGUILayout.EndVertical();
	}
	
	private void RebuildWalls()
	{
		Building building 					= (Building)target;
		
		string wallsID 						= building.BuildingName + "_" + Building.s_walls_id;
		
		GameObject wallsObject 				= GameObjectHelper.FindChild(building.gameObject, wallsID, true);
		wallsObject.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		
		GameObjectUtility.SetStaticEditorFlags(wallsObject, StaticEditorFlags.NavigationStatic);
		
		MeshFilter filter 		= wallsObject.GetComponent<MeshFilter>();
		MeshRenderer renderer 	= wallsObject.GetComponent<MeshRenderer>();
		
		if(filter == null) 		{ filter	= wallsObject.AddComponent<MeshFilter>(); }
		if(renderer == null) 	{ renderer  = wallsObject.AddComponent<MeshRenderer>(); }
		
		string wallAssetName = building.BuildingName + "_" + Building.s_walls_id;
		
		UnityEngine.Object wallMesh 	= AssetHelper.Instance.FindAsset<Mesh>(wallAssetName);
		UnityEngine.Object wallMaterial = AssetHelper.Instance.FindAsset<Material>(wallAssetName);
		
		if(wallMesh != null)
		{
			filter.mesh = wallMesh as Mesh;	
		}
		
		if(wallMaterial != null)
		{
			renderer.material = wallMaterial as Material;	
		}
		else
		{
			Debug.Log("Material Missing: " + wallAssetName);	
		}
	}
	
	private void RebuildRooms()
	{
		Building building 					= (Building)target;
		GameObject roomsObject 				= GameObjectHelper.FindChild(building.gameObject, Building.s_rooms_id, true);
		roomsObject.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		
		foreach(var room in building.Rooms)
		{
			BuildRoomObject(room, roomsObject);
		}
	}
	
	private GameObject BuildRoomObject(Room room, GameObject parent)
	{
		Building building 					= (Building)target;
		
		string roomID = building.BuildingName + "_" + room.Name + "_" + Building.s_floor_id;
		
		GameObject roomObject 					= new GameObject(roomID);
		roomObject.transform.parent				= parent.transform;
		roomObject.transform.localPosition  	= Vector3.zero;
		roomObject.transform.localRotation		= Quaternion.identity;
		
		GameObject floorObject 					= new GameObject(Building.s_floor_id);
		floorObject.transform.parent 			= roomObject.transform;
		floorObject.transform.localPosition		= new Vector3(0.0f, -0.9f, 0.0f);
		floorObject.transform.localRotation		= Quaternion.identity;
		
		GameObject ambientObject				= new GameObject(Building.s_ambient_id);
		ambientObject.layer						= LayerMask.NameToLayer("Shadow");
		ambientObject.transform.parent 			= roomObject.transform;
		ambientObject.transform.localPosition	= new Vector3(0.0f, 1.0f, 0.0f);
		ambientObject.transform.localRotation		= Quaternion.identity;
		
		BuildFloor(room, floorObject);
		BuildAmbient(room, ambientObject);
		
		return roomObject;
	}
	
	private void BuildAmbient(Room room, GameObject ambientObject)
	{
		Building building 		= (Building)target;
	
		AmbientLightZone ambient	= ambientObject.AddComponent<AmbientLightZone>();
		MeshRenderer renderer 		= ambientObject.AddComponent<MeshRenderer>();
		MeshFilter filter			= ambientObject.AddComponent<MeshFilter>();
		ambientObject.AddComponent<DisableMeshInEditor>();
		
		ambient.TODMinColor			= room.TODMinColor;
		ambient.TODMaxColor			= room.TODMaxColor;
		
		string ambientMeshName 		= building.BuildingName + "_" + room.Name + "_" + Building.s_ambient_id;
		
		UnityEngine.Object ambientMesh 		= AssetHelper.Instance.FindAsset<Mesh>(ambientMeshName);
		UnityEngine.Object ambientMaterial 	= AssetHelper.Instance.GetAsset<Material>("Materials/Ambient.mat");
		UnityEngine.Object ambientTexture	= AssetHelper.Instance.FindAsset<Texture>(ambientMeshName);
		
		if(ambientMesh != null)
		{
			filter.mesh = ambientMesh as Mesh;	
		}
		else
		{
			Debug.Log("Mesh Missing: " + ambientMeshName);	
		}
		
		if(ambientMaterial != null)
		{
			Material ambientCopy = new Material(ambientMaterial as Material);
			
			if(ambientTexture != null)
			{
				ambientCopy.mainTexture = ambientTexture as Texture;	
			}
			else
			{
				Debug.Log("Lightmap Texture Missing: " + ambientMeshName);	
			}
			
			renderer.material = ambientCopy;
		}
		else
		{
			Debug.Log("Material Missing: " + Building.s_ambient_id);	
		}
	}
	
	private void BuildFloor(Room room, GameObject floorObject)
	{
		GameObjectUtility.SetStaticEditorFlags(floorObject, StaticEditorFlags.NavigationStatic);
		
		Building building 		= (Building)target;
		
		MeshRenderer renderer 	= floorObject.AddComponent<MeshRenderer>();
		MeshFilter filter		= floorObject.AddComponent<MeshFilter>();
		
		string floorMeshName 	= building.BuildingName + "_" + room.Name + "_" + Building.s_floor_id;
		
		UnityEngine.Object floorMesh 		= AssetHelper.Instance.FindAsset<Mesh>(floorMeshName);
		UnityEngine.Object floorMaterial 	= AssetHelper.Instance.FindAsset<Material>(floorMeshName);
		
		if(floorMesh != null)
		{
			filter.mesh = floorMesh as Mesh;	
		}
		else
		{
			Debug.Log("Mesh Missing: " + floorMeshName);	
		}
		
		if(floorMaterial != null)
		{
			renderer.material = floorMaterial as Material;
		}
		else
		{
			Debug.Log("Material Missing: " + floorMeshName);	
		}
	}
	
	private void BuildWeatherMask()
	{
		Building building 						= (Building)target;
		GameObject weatherObject				= GameObjectHelper.FindChild(building.gameObject, Building.s_weather_mask_id, true);
		string maskHeightSetting				= Settings.Instance.GetSetting("building_weather_mask_height");
		float maskHeight = 3.1f;
		
		float.TryParse(maskHeightSetting, out maskHeight);
		
		weatherObject.transform.localPosition 	= new Vector3(0.0f, maskHeight, 0.0f);	
		weatherObject.layer						= LayerMask.NameToLayer("Weather");
		
		MeshFilter filter 		= weatherObject.GetComponent<MeshFilter>();
		MeshRenderer renderer 	= weatherObject.GetComponent<MeshRenderer>();
		//DepthMasked depthMasked = weatherObject.GetComponent<DepthMasked>();
		
		if(filter == null) 		{ filter		= weatherObject.AddComponent<MeshFilter>(); }
		if(renderer == null) 	{ renderer		= weatherObject.AddComponent<MeshRenderer>(); }
		//if(depthMasked == null)	{ depthMasked	= weatherObject.AddComponent<DepthMasked>(); }
		
		string weatherMaskMeshName = building.BuildingName + "_" + Building.s_weather_mask_id;
		
		UnityEngine.Object maskMesh		= AssetHelper.Instance.FindAsset<Mesh>(weatherMaskMeshName);
		UnityEngine.Object maskMaterial	= AssetHelper.Instance.FindAsset<Material>(Building.s_weather_mask_mat_id);
		
		if(maskMesh != null)
		{
			filter.mesh = maskMesh as Mesh;	
		}
		else
		{
			Debug.Log("Mesh Missing: " + weatherMaskMeshName);	
		}
		
		if(maskMaterial != null)
		{
			renderer.sharedMaterial = maskMaterial as Material;	
		}
		else
		{
			Debug.Log("Material Missing: " + Building.s_weather_mask_mat_id);	
		}
	}
	
	// TODO: Move this elsewhere
	private void GenerateLightmap(Room room)
	{
		Building building = (Building)target;
		
		string outputPath = Application.dataPath + "/Resources/Textures/Structures/" + building.BuildingName + "/" + building.BuildingName + "_" + room.Name + "_" + Building.s_ambient_id + ".png";
		Debug.Log("Saving to outputPath: " + outputPath);
		
		LightMapGenerator.GenerateLightmap(building, room, outputPath, 2, true);
	}

}
