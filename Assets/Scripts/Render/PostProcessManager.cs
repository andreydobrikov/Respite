using UnityEngine;
using System.Collections;

public class PostProcessManager : MonoBehaviour 
{
	public bool ViewRegionEnabled = false;
	
	void Start () 
	{
		GameObject lightmapCamera 	= GameObject.FindGameObjectWithTag("LightMapCamera");
		GameObject targetCamera 	= GameObject.FindGameObjectWithTag("WeatherCamera");
		GameObject postCamera		= GameObject.FindGameObjectWithTag("PostCamera");
		GameObject postPostCamera		= GameObject.FindGameObjectWithTag("PostPostCamera");
		GameObject overlayCamera	= GameObject.FindGameObjectWithTag("OverlayCamera");
		GameObject viewCamera		= GameObject.FindGameObjectWithTag("ViewRegionCamera");
		
		if(lightmapCamera != null && targetCamera != null )
		{
			
			// Fiddle with these to use a smaller render-texture for the light-pass.
			// Note: Too small a target can cause light bleeding when the overlay is interpolated.
			int pixelWidth 	= (int)Camera.mainCamera.pixelWidth;
			int pixelHeight = (int)Camera.mainCamera.pixelHeight;
			
			if(true)
			{
				
				// If this is run-in-editor, the camera's aspect ratio will not yet have been updated.
				// This in turn will crap up the aspect ratio of the attached RenderTexture, so manually set the aspect
				// - ratio before creating the texture.
				lightmapCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
				lightmapCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
				lightmapCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
				targetCamera.GetComponent<LightMapEffect>().lightMapTexture 		= lightmapCamera.GetComponent<Camera>().targetTexture;
			}
			
			if(ViewRegionEnabled)
			{
				if(viewCamera != null && postCamera != null)
				{
					int viewWidth 													= pixelWidth;
					int viewHeight 													= pixelHeight;
					viewCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
					viewCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(viewWidth, viewHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
					viewCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
					
					postCamera.GetComponent<ViewRegionEffect>().viewRegionTexture 	= viewCamera.GetComponent<Camera>().targetTexture;
					
					postPostCamera.GetComponent<BlurEffect>().OverlayTexture			= viewCamera.GetComponent<Camera>().targetTexture;
				}
				else
				{
					Debug.Log("View or Post cameras not enabled");	
				}
			}
		}
	}
}
