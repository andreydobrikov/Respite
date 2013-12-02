using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Light Map")]
[RequireComponent(typeof(Camera))]

public class LightMapEffect : ImageEffectBase
{
	public RenderTexture lightMapTexture;
	public RenderTexture targetTexture = null;
	
	override protected void Start()
	{
		if(!SystemInfo.supportsRenderTextures)
		{
			enabled = false;
			return;
		}
		base.Start();
	}
	
	override protected void OnDisable()
	{
		base.OnDisable();
//		DestroyImmediate(lightMapTexture);
	}
	
	public void UpdateShader()
	{
		material.shader = shader;		
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		// Create the accumulation texture
		if (lightMapTexture == null)
		{
			DestroyImmediate(lightMapTexture);
		}
	
		
		
		material.SetTexture("_Overlay", lightMapTexture);
		
		
		Graphics.Blit(source, destination, material);
	}
}
