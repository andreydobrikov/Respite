using UnityEngine;
using System.Collections;

public class PostProcessManager : MonoBehaviour 
{
	public bool ViewRegionEnabled = false;
	
	void Start () 
	{
		GameObject mainCamera	 	= GameObject.FindGameObjectWithTag("MainCamera");
		GameObject lightmapCamera 	= GameObject.FindGameObjectWithTag("LightMapCamera");
		GameObject targetCamera 	= GameObject.FindGameObjectWithTag("WeatherCamera");
		GameObject postCamera		= GameObject.FindGameObjectWithTag("PostCamera");
		GameObject postPostCamera	= GameObject.FindGameObjectWithTag("PostPostCamera");
		GameObject overlayCamera	= GameObject.FindGameObjectWithTag("OverlayCamera");
		GameObject viewCamera		= GameObject.FindGameObjectWithTag("ViewRegionCamera");
		GameObject altWorldCamera	= GameObject.FindGameObjectWithTag("AltWorldCamera");
		
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
			
			int viewWidth 	= pixelWidth;
			int viewHeight 	= pixelHeight;
			
			if(altWorldCamera != null)
			{
				altWorldCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
				altWorldCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(viewWidth, viewHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
				altWorldCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
			}
			
			if(ViewRegionEnabled)
			{
				if(viewCamera != null && postCamera != null)
				{
					
					viewCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
					viewCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(viewWidth, viewHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
					viewCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
					
					postCamera.GetComponent<ViewRegionEffect>().viewRegionTexture 	= viewCamera.GetComponent<Camera>().targetTexture;
					
//					postPostCamera.GetComponent<BlurEffect>().OverlayTexture			= viewCamera.GetComponent<Camera>().targetTexture;
					
					if(altWorldCamera != null)
					{
						mainCamera.GetComponent<AltWorldEffect>().altWorldTexture 	= altWorldCamera.GetComponent<Camera>().targetTexture;
						mainCamera.GetComponent<AltWorldEffect>().viewTexture		= viewCamera.GetComponent<Camera>().targetTexture;
					}
					
				}
				else
				{
					Debug.Log("View or Post cameras not enabled");	
				}
			}
		}
	}
}
