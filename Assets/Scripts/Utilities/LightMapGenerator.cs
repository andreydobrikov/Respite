///////////////////////////////////////////////////////////
// 
// LightMapGenerator.cs
//
// What it does: Creates the rather crude lightmap textures I use to imply some ambient-occlusion from walls
//
// Notes: This is tied to the Building code fairly tightly.                    
// 
// To-do:
//
///////////////////////////////////////////////////////////

using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class LightMapGenerator 
{

	public static void GenerateLightmap(Building building, Room room, string textureOutputPath, int pixelSpread, bool filter)
	{
		List<IntersectionLine> m_intersections = new List<IntersectionLine>();
		
		string roomID 	= building.BuildingName + "_" + room.Name + "_" + Building.s_floor_id;
		string wallsID	= building.BuildingName + "_" + Building.s_walls_id;
		
		GameObject roomsObject 	= GameObjectHelper.FindChild(building.gameObject, Building.s_rooms_id, false);
		GameObject wallsObject 	= GameObjectHelper.FindChild(building.gameObject, wallsID, false);
		
		// TODO: Name this something else. Very confusing having roomObject and roomsObject
		GameObject roomObject = GameObjectHelper.FindChild(roomsObject, roomID, false);
		GameObject floorObject = GameObjectHelper.FindChild(roomObject, Building.s_floor_id, false);
		GameObject ambientObject = GameObjectHelper.FindChild(roomObject, Building.s_ambient_id, false);
		
		if(floorObject != null)
		{
			Debug.Log("Found: " + roomID);	
		}
		else
		{
			Debug.Log("Couldn't find room: " + roomID);
			return;
		}
		
		if(wallsObject != null)
		{
			Debug.Log("Found: " + wallsID);	
		}
		else
		{
			Debug.Log("Couldn't find walls: " + wallsID);
			return;
		}
		
		MeshFilter ambientFilter = ambientObject.GetComponent<MeshFilter>();
		MeshFilter roomFilter	= floorObject.GetComponent<MeshFilter>();
		MeshFilter wallsFilter	= wallsObject.GetComponent<MeshFilter>();
		
		if(ambientFilter == null) { Debug.LogWarning("Ambient Filter missing for room: " + room.Name); return; } 
		if(roomFilter == null) { Debug.LogWarning("Room Filter missing for room: " + room.Name); return; }
		if(wallsFilter == null) { Debug.LogWarning("Walls Filter missing for room: " + room.Name); return; }
		
		Mesh ambientMesh = ambientFilter.sharedMesh;
		Mesh roomMesh 	= roomFilter.sharedMesh;
		Mesh wallsMesh 	= wallsFilter.sharedMesh;
		
		if(ambientMesh == null) { Debug.LogWarning("Ambient Mesh missing for room: " + room.Name); return; } 
		if(roomMesh == null) { Debug.LogWarning("Room Mesh missing for room: " + room.Name); return; }
		if(wallsMesh == null) { Debug.LogWarning("Walls Mesh missing for room: " + room.Name); return; }
		
		m_intersections.Clear();
		
		int triangleCount = wallsMesh.triangles.Length / 3;
		
		for(int tri = 0; tri < triangleCount; ++tri)
		{
			IntersectionLine newLine0 = new IntersectionLine();
			IntersectionLine newLine1 = new IntersectionLine();
			IntersectionLine newLine2 = new IntersectionLine();
			
			newLine0.start 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3]];
			newLine0.end 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3 + 1]];
			
			newLine1.start 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3 + 1]];
			newLine1.end 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3 + 2]];
			
			newLine2.start 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3 + 2]];
			newLine2.end 	= wallsMesh.vertices[wallsMesh.triangles[tri * 3 ]];
			
			int triangleCountAlt = roomMesh.triangles.Length / 3;
			
			for(int triAlt = 0; triAlt < triangleCountAlt; ++triAlt)
			{
				IntersectionLine newLine0Alt = new IntersectionLine();
				IntersectionLine newLine1Alt = new IntersectionLine();
				IntersectionLine newLine2Alt = new IntersectionLine();
				
				newLine0Alt.start 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3]];
				newLine0Alt.end 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 1]];
				
				newLine1Alt.start 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 1]];
				newLine1Alt.end 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 2]];
				
				newLine2Alt.start 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 2]];
				newLine2Alt.end 	= roomMesh.vertices[roomMesh.triangles[triAlt * 3 ]];
				
				// BAH!
				Vector2 v0 = new Vector2(newLine0.start.x, newLine0.start.z);
				Vector2 v1 = new Vector2(newLine0.end.x, newLine0.end.z);
				
				Vector2 v2 = new Vector2(roomMesh.vertices[roomMesh.triangles[triAlt * 3]].x, roomMesh.vertices[roomMesh.triangles[triAlt * 3]].z);
				Vector2 v3 = new Vector2(roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 1]].x, roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 1]].z);
				Vector2 v4 = new Vector2(roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 2]].x, roomMesh.vertices[roomMesh.triangles[triAlt * 3 + 2]].z);
				
				Vector2 intersection0, intersection1;
				
				bool intersect = MathsHelper.LineTriIntersect(v0, v1, v2, v3, v4, out intersection0, out intersection1);
				intersect |= MathsHelper.LineInTri(v0, v1, v2, v3, v4);
				
				if(intersect) { m_intersections.Add(newLine0); }
				
				v0 = new Vector2(newLine1.start.x, newLine1.start.z);
				v1 = new Vector2(newLine1.end.x, newLine1.end.z);
				
				intersect = MathsHelper.LineTriIntersect(v0, v1, v2, v3, v4, out intersection0, out intersection1);
				intersect |= MathsHelper.LineInTri(v0, v1, v2, v3, v4);
				
				if(intersect) { m_intersections.Add(newLine1); }
				
				v0 = new Vector2(newLine2.start.x, newLine2.start.z);
				v1 = new Vector2(newLine2.end.x, newLine2.end.z);
				
				intersect = MathsHelper.LineTriIntersect(v0, v1, v2, v3, v4, out intersection0, out intersection1);
				intersect |= MathsHelper.LineInTri(v0, v1, v2, v3, v4);
				
				if(intersect) { m_intersections.Add(newLine2); }
				
			}
		}
		
		// Create a texture, innit
		
		int mipLevel = 0;
		
		const int texWidth = 256;
		const int texHeight = 256;
		
		int mipWidth = texWidth >> mipLevel;
		int mipHeight	= texHeight >> mipLevel;
		
		Debug.Log("Mip: " + mipWidth +", " + mipHeight);
		
		Texture2D testTexture = new Texture2D(mipWidth, mipHeight);
		
		float texelWidth = ambientMesh.bounds.size.x / (float)mipWidth;
		float texelHeight = ambientMesh.bounds.size.y / (float)mipHeight;
		
		for(int x = 0; x < mipWidth; ++x)
		{
			for(int y = 0; y < mipHeight; ++y)
			{
				testTexture.SetPixel(x, y,  new Color(1.0f, 1.0f, 1.0f, 1.0f));
			}	
		}
		
		foreach(var intersection in m_intersections)
		{
			float length = (intersection.end - intersection.start).magnitude;
			
			Vector3 localStart 	= intersection.start - ambientMesh.bounds.min;
			Vector3 localEnd 	= intersection.end - ambientMesh.bounds.min;
			
			// trundle along the line and draw some things
			
			float delta = 1.0f / 1000.0f;
			
			for(int i = 0; i < 1000; i++)
			{
				for(int spreadX = -(pixelSpread / 2); spreadX < pixelSpread / 2; spreadX++)
				{
					for(int spreadY = -(pixelSpread / 2); spreadY < pixelSpread / 2; spreadY++)
					{
						Vector3 currentPixel = (localStart + (delta * i) * (localEnd - localStart));
						
						currentPixel.x = currentPixel.x / ambientMesh.bounds.size.x;
						currentPixel.z = currentPixel.z / ambientMesh.bounds.size.z;
						
						currentPixel.x *= mipWidth;
						currentPixel.z *= mipHeight;
						
						currentPixel.x += spreadX;
						currentPixel.z += spreadY;
						
						currentPixel.x = (float)mipWidth - currentPixel.x;
						
						if(currentPixel.x > 0.0f && currentPixel.z > 0.0f && currentPixel.x < mipWidth && currentPixel.z < mipHeight)
						{
							testTexture.SetPixel((int)currentPixel.x, (int)currentPixel.z, Color.black);
						}
					}	
				}
			}
		}
		
		testTexture.EncodeToPNG();
		
		string directory = System.IO.Path.GetDirectoryName(textureOutputPath);
		
		if(!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);	
		}
		
		System.IO.File.WriteAllBytes(textureOutputPath, testTexture.EncodeToPNG());
		
		string relative = textureOutputPath.Remove(textureOutputPath.IndexOf(Application.dataPath), Application.dataPath.Length);
		
		AssetDatabase.ImportAsset(relative, ImportAssetOptions.ForceSynchronousImport);
	}
	
	private struct IntersectionLine
	{
		public Vector3 start;
		public Vector3 end;
	}
	
}
