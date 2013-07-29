using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Overlay Effect")]
[RequireComponent(typeof(Camera))]

public class OverlayEffect : ImageEffectBase
{
	public Texture		 BlendTexture;
	public RenderTexture OverlayTexture;
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

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		// Create the accumulation texture
		if (OverlayTexture == null)
		{
			DestroyImmediate(OverlayTexture);
		}
	
		material.SetTexture("_Overlay", OverlayTexture);
		material.SetTexture("_Blend", BlendTexture);
		
		
		Graphics.Blit(source, targetTexture, material);
	}
}
