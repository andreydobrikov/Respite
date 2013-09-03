#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class PostProcessManager : MonoBehaviour 
{
	public bool ViewRegionEnabled = false;
	
	public Shader ShowLightmapShader = null;
	
	void Start () 
	{
		GameObject lightmapCamera 	= GameObject.FindGameObjectWithTag("LightMapCamera");
		GameObject targetCamera 	= GameObject.FindGameObjectWithTag("WeatherCamera");
		GameObject postCamera		= GameObject.FindGameObjectWithTag("PostCamera");
		GameObject viewCamera		= GameObject.FindGameObjectWithTag("ViewRegionCamera");
		
		if(postCamera != null)
		{
			m_postBlur = postCamera.GetComponent<BlurEffect>() as BlurEffect;	
		}
		
		m_lightMapEffect = FindObjectOfType(typeof(LightMapEffect)) as LightMapEffect;
		
		if(lightmapCamera != null && targetCamera != null )
		{
			
			// Fiddle with these to use a smaller render-texture for the light-pass.
			// Note: Too small a target can cause light bleeding when the overlay is interpolated.
			int pixelWidth 	= (int)Camera.mainCamera.pixelWidth;
			int pixelHeight = (int)Camera.mainCamera.pixelHeight;
			
			// If this is run-in-editor, the camera's aspect ratio will not yet have been updated.
			// This in turn will crap up the aspect ratio of the attached RenderTexture, so manually set the aspect
			// - ratio before creating the texture.
			lightmapCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
			lightmapCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
			lightmapCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
			targetCamera.GetComponent<LightMapEffect>().lightMapTexture 		= lightmapCamera.GetComponent<Camera>().targetTexture;
		
			int viewWidth 	= pixelWidth;
			int viewHeight 	= pixelHeight;
			
			if(ViewRegionEnabled)
			{
				if(viewCamera != null && postCamera != null)
				{
					
					viewCamera.GetComponent<Camera>().aspect 						= (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
					viewCamera.GetComponent<Camera>().targetTexture 				= new RenderTexture(viewWidth, viewHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
					viewCamera.GetComponent<Camera>().targetTexture.isPowerOfTwo 	= false;
					
					postCamera.GetComponent<ViewRegionEffect>().viewRegionTexture 	= viewCamera.GetComponent<Camera>().targetTexture;
					
				}
				else
				{
					Debug.Log("View or Post cameras not enabled");	
				}
			}
		}
	}
	
	void Update()
	{
		if(m_blurUp)
		{
			m_blurLerpProgress += Time.deltaTime;	
		}
		else
		{
			m_blurLerpProgress -= Time.deltaTime;	
		}
		
		m_blurLerpProgress = Mathf.Clamp(m_blurLerpProgress, 0.0f, 0.6f);
		
		if(m_postBlur != null)
		{
			m_postBlur.blurSpread = m_blurLerpProgress;	
		}
	}	
	
	public void ActivateBlur()
	{
		m_blurRequests++;
		
		if(m_postBlur != null )
		{
			m_postBlur.enabled = true;
		}
	}
	
	public void DeactivateBlur()
	{
		m_blurRequests--;
		
		if(m_blurRequests == 0)
		{
			if(m_postBlur != null)
			{
				m_postBlur.enabled = false;
			}
		}
			
		if(m_blurRequests < 0)
		{
			Debug.LogError("Invalid number of blur-requests");	
		}
	}
	
#if UNITY_EDITOR
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(600, 10, 180, 100));
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Post Process");
		
		if(m_showFoldout)
		{
			GUI.enabled = m_lightMapEffect != null;
			
			bool showLightmap = GUILayout.Toggle(m_showLightmap, "Show Light-Map");
			
			if(showLightmap != m_showLightmap && m_lightMapEffect != null)
			{
				if(showLightmap)
				{
					m_defaultLightMapTexture = m_lightMapEffect.shader;	
					m_lightMapEffect.shader = ShowLightmapShader;
					m_lightMapEffect.UpdateShader();
				}
				else
				{
					m_lightMapEffect.shader = m_defaultLightMapTexture;
					m_lightMapEffect.UpdateShader();
				}
				
				m_showLightmap = showLightmap;
			}
			
			GUI.enabled = true;	
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
#endif
	
	private int m_blurRequests				= 0;
	private float m_blurLerpProgress		= 0.0f;
	private bool m_blurUp					= false;
	private BlurEffect m_postBlur 			= null;
	private Shader m_defaultLightMapTexture = null;
	private bool m_showLightmap 			= false;
	private bool m_showFoldout 				= false;
	private LightMapEffect m_lightMapEffect = null;
}
