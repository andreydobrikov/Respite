using UnityEngine;

// This class implements simple ghosting type Motion Blur.
// If Extra Blur is selected, the scene will allways be a little blurred,
// as it is scaled to a smaller resolution.
// The effect works by accumulating the previous frames in an accumulation
// texture.
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Light Map")]
[RequireComponent(typeof(Camera))]

public class LightMapEffect : ImageEffectBase
{
	public RenderTexture lightMapTexture;
	
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
		if (lightMapTexture == null)
		{
			DestroyImmediate(lightMapTexture);
		}
		
		// Setup the texture and floating point values in the shader
		material.SetTexture("_Overlay", lightMapTexture);
		
		// Render the image using the motion blur shader
		Graphics.Blit (source, null, material);
	}
}
