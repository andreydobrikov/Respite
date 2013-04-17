/// <summary>
/// Sprite.cs
/// 
/// The Sprite class is the editor-facing sprite object used to determine data and sprite parameters.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Sprite : MonoBehaviour 
{
	public string SpriteData 		= "";
	public Texture2D SpriteTexture 	= null;
	public float updateSpeed 		= 0.1f;
	
	private SpriteData m_data;
	private float timeProgress = 0.0f;
		
	void Start () 
	{
		if(LoadSpriteData(SpriteData))
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			
			meshFilter.sharedMesh = CreateSpritePlane();
			meshRenderer.material.mainTexture = SpriteTexture;
		}
		else
		{
			Debug.LogError("Failed to load sprite data: " + SpriteData);	
		}
	}
	
	void Update () 
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		
		timeProgress += Time.deltaTime;
		if(timeProgress > updateSpeed)
		{
			timeProgress = 0.0f;
			if(m_data != null && m_data.CurrentAnimation != null)
			{
				Vector4 offset = m_data.CurrentAnimation.Advance();
				meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offset.x, offset.y));
				meshRenderer.material.SetTextureScale("_MainTex", new Vector2(offset.z, offset.w));
				
			}
		}
	}
	
	public bool LoadSpriteData(string spriteData)
	{
		m_data = new SpriteData();
		m_data.SetTexture(SpriteTexture);
		
		return m_data.Load(spriteData);
	}
	
	private Mesh CreateSpritePlane()
	{
		Mesh newMesh = new Mesh();
		
		newMesh.name = "GeometryFactory:Plane";
		
		Vector3[] 	vertices 	= new Vector3[4];
		Vector2[] 	uvs 		= new Vector2[4];
		int[] 		triangles 	= new int[6];
		
		vertices[0] = new Vector3(-0.5f, -0.5f, 0.0f);
		vertices[1] = new Vector3(0.5f, -0.5f, 0.0f);
		vertices[2] = new Vector3(-0.5f, 0.5f, 0.0f);
		vertices[3] = new Vector3(0.5f, 0.5f, 0.0f);
		
		uvs[0] = new Vector2(0.0f, 0.0f);
		uvs[1] = new Vector2(1.0f, 0.0f);
		uvs[2] = new Vector2(0.0f, 1.0f);
		uvs[3] = new Vector2(1.0f, 1.0f);
		
		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		
		newMesh.vertices 	= vertices;
		newMesh.uv 			= uvs;
		newMesh.triangles 	= triangles;
		
		return newMesh;
	}
}


