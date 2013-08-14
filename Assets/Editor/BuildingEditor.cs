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
		
		for(int roomID = 0; roomID < building.Rooms.Count; ++roomID)
		{
			Room current = building.Rooms[roomID];
			
			EditorGUILayout.BeginHorizontal();
			
			
			current.ShowFoldout = EditorGUILayout.Foldout(current.ShowFoldout, current.Name);
			
			if(GUILayout.Button("Delete"))
			{
				// TODO: er, delete.	
			}
			
			EditorGUILayout.EndHorizontal();
			
			if(current.ShowFoldout)
			{
				EditorGUILayout.BeginVertical((GUIStyle)("Box"));
				
				EditorGUI.indentLevel = 1;
				
				current.Name = EditorGUILayout.TextField(current.Name);
				current.TODMaxColor = EditorGUILayout.ColorField("TOD Max Colour", current.TODMaxColor);
				current.TODMinColor = EditorGUILayout.ColorField("TOD Min Colour", current.TODMinColor);
				
				EditorGUI.indentLevel = 0;
				
				EditorGUILayout.EndVertical();
			}
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
		
		EditorGUILayout.EndVertical();
	}
	
	private void RebuildWalls()
	{
		Building building 					= (Building)target;
		GameObject wallsObject 				= GameObjectHelper.FindChild(building.gameObject, s_walls_id, true);
		wallsObject.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
		
		MeshFilter filter 		= wallsObject.GetComponent<MeshFilter>();
		MeshRenderer renderer 	= wallsObject.GetComponent<MeshRenderer>();
		
		if(filter == null) 		{ filter	= wallsObject.AddComponent<MeshFilter>(); }
		if(renderer == null) 	{ renderer  = wallsObject.AddComponent<MeshRenderer>(); }
		
		string wallAssetName = building.BuildingName + "_" + s_walls_id;
		
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
	}
	
	private void RebuildRooms()
	{
		Building building 					= (Building)target;
		GameObject roomsObject 				= GameObjectHelper.FindChild(building.gameObject, s_rooms_id, true);
		roomsObject.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
		
		foreach(var room in building.Rooms)
		{
			BuildRoomObject(room, roomsObject);
		}
	}
	
	private GameObject BuildRoomObject(Room room, GameObject parent)
	{
		GameObject roomObject 					= new GameObject(room.Name);
		roomObject.transform.parent				= parent.transform;
		roomObject.transform.localPosition  	= Vector3.zero;
		
		GameObject floorObject 					= new GameObject(s_floor_id);
		floorObject.transform.parent 			= roomObject.transform;
		floorObject.transform.localPosition		= new Vector3(0.0f, 0.0f, 0.9f);
		
		GameObject ambientObject				= new GameObject(s_ambient_id);
		ambientObject.layer						= LayerMask.NameToLayer("Lights");
		ambientObject.transform.parent 			= roomObject.transform;
		ambientObject.transform.localPosition	= new Vector3(0.0f, 0.0f, -1.0f);
		
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
		
		string ambientMeshName 		= building.BuildingName + "_" + room.Name + "_" + s_ambient_id;
		
		UnityEngine.Object ambientMesh 		= AssetHelper.Instance.FindAsset<Mesh>(ambientMeshName);
		UnityEngine.Object ambientMaterial 	= AssetHelper.Instance.GetAsset<Material>("Materials/Ambient.mat");
		
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
			renderer.material = ambientMaterial as Material;
		}
		else
		{
			Debug.Log("Material Missing: " + s_ambient_id);	
		}
	}
	
	private void BuildFloor(Room room, GameObject floorObject)
	{
		Building building 		= (Building)target;
		
		MeshRenderer renderer 	= floorObject.AddComponent<MeshRenderer>();
		MeshFilter filter		= floorObject.AddComponent<MeshFilter>();
		
		string floorMeshName 	= building.BuildingName + "_" + room.Name + "_" + s_floor_id;
		
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
		GameObject weatherObject				= GameObjectHelper.FindChild(building.gameObject, s_weather_mask_id, true);
		weatherObject.transform.localPosition 	= new Vector3(0.0f, 0.0f, -1.0f);	
		
		MeshFilter filter 		= weatherObject.GetComponent<MeshFilter>();
		MeshRenderer renderer 	= weatherObject.GetComponent<MeshRenderer>();
		DepthMasked depthMasked = weatherObject.GetComponent<DepthMasked>();
		
		if(filter == null) 		{ filter		= weatherObject.AddComponent<MeshFilter>(); }
		if(renderer == null) 	{ renderer		= weatherObject.AddComponent<MeshRenderer>(); }
		if(depthMasked == null)	{ depthMasked	= weatherObject.AddComponent<DepthMasked>(); }
		
		string weatherMaskMeshName = building.BuildingName + "_" + s_weather_mask_id;
		
		UnityEngine.Object maskMesh		= AssetHelper.Instance.FindAsset<Mesh>(weatherMaskMeshName);
		UnityEngine.Object maskMaterial	= AssetHelper.Instance.FindAsset<Material>(s_weather_mask_mat_id);
		
		if(maskMesh != null)
		{
			filter.mesh = maskMesh as Mesh;	
		}
		else
		{
			Debug.Log("Mesh Missing: " + maskMesh);	
		}
		
		if(maskMaterial != null)
		{
			renderer.sharedMaterial = maskMaterial as Material;	
		}
		else
		{
			Debug.Log("Material Missing: " + s_weather_mask_mat_id);	
		}
	}
	
	private static string s_walls_id 			= "walls";
	private static string s_rooms_id 			= "rooms";
	private static string s_floor_id 			= "floor";
	private static string s_ambient_id 			= "ambient";
	private static string s_lightmap_id 		= "lightmap";
	private static string s_weather_mask_id 	= "weather_mask";
	private static string s_weather_mask_mat_id = "DepthMask";
}
