using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/View Region")]
[RequireComponent(typeof(Camera))]

public class ViewRegionEffect : ImageEffectBase
{
	public RenderTexture viewRegionTexture;
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
		if (viewRegionTexture == null)
		{
			DestroyImmediate(viewRegionTexture);
		}
	
		material.SetTexture("_Overlay", viewRegionTexture);
		
		
		Graphics.Blit(source, targetTexture, material);
	}
}
