using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/AltWorld")]
[RequireComponent(typeof(Camera))]

public class AltWorldEffect : ImageEffectBase
{
	public RenderTexture viewTexture;
	public RenderTexture altWorldTexture;
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
		if (altWorldTexture == null)
		{
			DestroyImmediate(altWorldTexture);
		}
	
		material.SetTexture("_Overlay", altWorldTexture);
		material.SetTexture("_View", viewTexture);
		
		
		Graphics.Blit(source, targetTexture, material);
	}
}
