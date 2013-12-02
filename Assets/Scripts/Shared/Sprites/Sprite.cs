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
	public bool UseDebugColours		= false;
	public bool BlendFrames 		= false;
	public string SpriteData 		= "";
	public Texture2D SpriteTexture 	= null;
	public float updateSpeed 		= 0.1f;
	
	private SpriteAnimationSet m_data;
	private float timeProgress = 0.0f;
		
	void Start () 
	{
		if(LoadSpriteData(SpriteData))
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			
			meshFilter.sharedMesh = CreateSpritePlane();
			meshRenderer.material.SetTexture("_SpriteTex0", SpriteTexture);
			meshRenderer.material.SetTexture("_SpriteTex1", SpriteTexture);
			
			m_spriteMaterial = meshRenderer.material;
		}
		else
		{
			Debug.LogError("Failed to load sprite data: " + SpriteData);	
		}
	}
	
	void Update () 
	{
		if(m_spriteMaterial == null)
		{
			return;	
		}
		
		
		timeProgress += Time.deltaTime;
		m_spriteMaterial.SetFloat("_BlendFrameLerp", 1.0f - ( timeProgress / updateSpeed));
		
		if(timeProgress > updateSpeed)
		{
			timeProgress = 0.0f;
			UpdateShader();
		}
		//Debug.Log("LERP: " + timeProgress / updateSpeed + "( " + timeProgress + " \\ " + updateSpeed + " ) ");

	}
	
	public void Play(string animationName)
	{
		m_data.PlayAnimation(animationName);	
		UpdateShader();
	}
	
	public bool LoadSpriteData(string spriteData)
	{
		m_data = new SpriteAnimationSet();
		m_data.SpriteSetTexture = SpriteTexture;
		
		return m_data.Load(spriteData);
	}
	
	private void UpdateShader()
	{
		m_spriteMaterial.SetFloat("_BlendFrameLerp", 1.0f -timeProgress / updateSpeed);
		if(m_data != null && m_data.CurrentAnimation != null)
		{
			m_previousOffset = m_data.CurrentAnimation.CurrentOffset();
			
			Vector4 offset = m_data.CurrentAnimation.Advance();
			m_spriteMaterial.SetTextureOffset("_SpriteTex0", new Vector2(offset.x, offset.y));
			m_spriteMaterial.SetTextureScale("_SpriteTex0", new Vector2(offset.z, offset.w));
			
			if(UseDebugColours)
			{
				Vector3 color = m_data.CurrentAnimation.DebugColour;
				Color debugColor = new Color(color.x, color.y, color.z, 1.0f);
				m_spriteMaterial.SetColor("_Color", debugColor);
			}
			
			if(BlendFrames)
			{
				m_spriteMaterial.SetTextureOffset("_SpriteTex1", new Vector2(m_previousOffset.x, m_previousOffset.y));
				m_spriteMaterial.SetTextureScale("_SpriteTex1", new Vector2(m_previousOffset.z, m_previousOffset.w));
			}
			else
			{
				m_spriteMaterial.SetTextureOffset("_SpriteTex1", new Vector2(offset.x, offset.y));
				m_spriteMaterial.SetTextureScale("_SpriteTex1", new Vector2(offset.z, offset.w));	
			}
		}
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
	
	private Vector4 m_previousOffset = Vector4.one;
	private Material m_spriteMaterial;
}


