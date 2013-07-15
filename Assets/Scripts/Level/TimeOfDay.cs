using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour 
{
	public float CycleTime 				= 1000.0f;
	public float ActiveTime 			= 0.0f;
	public float StartTime 				= 0.0f;
	public bool PauseUpdate 			= false;
	public float CloudCoverPercentage 	= 0.0f;
	public Vector4 TODColor 			= Vector4.one;
	
	void Start()
	{
		Reset ();
	}
	
	private void Reset()
	{
		GameObject cameraObject = GameObject.FindGameObjectWithTag("LightMapCamera");
		GameObject targetCamera = GameObject.FindGameObjectWithTag("WeatherCamera");
		GameObject postCamera	= GameObject.FindGameObjectWithTag("PostCamera");
		GameObject viewCamera 	= GameObject.FindGameObjectWithTag("ViewRegionCamera");
		
		if(cameraObject != null && targetCamera != null && viewCamera != null)
		{
			m_lightMapCamera 	= cameraObject.GetComponent<Camera>();
			Camera viewRegionCamera 	= viewCamera.GetComponent<Camera>();
			
			// Fiddle with these to use a smaller render-texture for the light-pass.
			// Note: Too small a target can cause light bleeding when the overlay is interpolated.
			int pixelWidth 	= (int)Camera.mainCamera.pixelWidth;
			int pixelHeight = (int)Camera.mainCamera.pixelHeight;
			
			if(true)
			{
				// If this is run-in-editor, the camera's aspect ratio will not yet have been updated.
				// This in turn will crap up the aspect ratio of the attached RenderTexture, so manually set the aspect
				// - ratio before creating the texture.
				m_lightMapCamera.aspect = (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
				
				m_lightMapCamera.targetTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
				m_lightMapCamera.targetTexture.isPowerOfTwo = false;
				targetCamera.GetComponent<LightMapEffect>().lightMapTexture = m_lightMapCamera.targetTexture;
			}
			
			int viewWidth 	= pixelWidth;
			int viewHeight 	= pixelHeight;
			viewRegionCamera.aspect = (Camera.mainCamera.pixelWidth / Camera.mainCamera.pixelHeight);
			viewRegionCamera.targetTexture = new RenderTexture(viewWidth, viewHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.sRGB);
			viewRegionCamera.targetTexture.isPowerOfTwo = false;
			postCamera.GetComponent<ViewRegionEffect>().viewRegionTexture = viewRegionCamera.targetTexture;
			
		}
		 
		ActiveTime 			= StartTime;
		m_currentFrameIndex = 0;
		
		TODKeyFrame frame1 = new TODKeyFrame();
		
		if(m_frames == null || m_frames.Count == 0)
		{
			m_frames = new List<TODKeyFrame>();
			m_frames.Add(frame1);	 
		}
		
		AdvanceFrame();
		UpdateTime(true);
	}
	
	void FixedUpdate () 
	{
		if(!PauseUpdate)
		{
			ActiveTime += Time.deltaTime; 
		}
		
		//Debug.Log("Width: " + (int)Camera.mainCamera.pixelWidth);	
		//Debug.Log("Height: " + (int)Camera.mainCamera.pixelHeight);	
		
		UpdateTime(false);
	}
	
	public void UpdateTime(bool recalculateFrame)
	{
		float adjustedTime = ActiveTime / CycleTime;
		
		if(recalculateFrame)
		{
			for(int frameIndex = 0; frameIndex < m_frames.Count - 1; frameIndex++)
			{
				if(m_frames[frameIndex].FrameTime < ActiveTime && m_frames[frameIndex + 1].FrameTime > ActiveTime)
				{
					m_currentFrameIndex = frameIndex;
					AdvanceFrame();
					break;
				}
			}
			
			adjustedTime = ActiveTime;
		}
		
		if( (m_nextFrame.FrameTime > m_currentFrame.FrameTime && adjustedTime > m_nextFrame.FrameTime) ||
			(m_nextFrame.FrameTime < m_currentFrame.FrameTime && (adjustedTime > m_nextFrame.FrameTime && adjustedTime < m_currentFrame.FrameTime)))
		{
			AdvanceFrame();	
		}
		
		if(ActiveTime > CycleTime)
		{
			ActiveTime = 0.0f;	
		}
		
		float timeSeparation 	= m_nextFrame.FrameTime - m_currentFrame.FrameTime;
		float progress 			= (adjustedTime - m_currentFrame.FrameTime) / timeSeparation;
		Vector4 lerpedValue 	= Vector4.Lerp(m_currentFrame.FrameColor, m_nextFrame.FrameColor, progress);
				
		// This is required for looping to work
		if(m_nextFrame.FrameTime < m_currentFrame.FrameTime)
		{
			timeSeparation = (1.0f - m_currentFrame.FrameTime) + m_nextFrame.FrameTime;		
			
			if( adjustedTime > m_currentFrame.FrameTime)
			{
				progress = (adjustedTime - m_currentFrame.FrameTime) / timeSeparation;		
			}
			else
			{
				float leftover = 1.0f - m_currentFrame.FrameTime;
				progress = (leftover + adjustedTime) / timeSeparation;
			}
			
			// As a cheat, LERP backwards with 1.0f - progress
			lerpedValue = Vector4.Lerp(m_nextFrame.FrameColor, m_currentFrame.FrameColor, 1.0f - progress);
		}
		
		lerpedValue *= (1.0f - (CloudCoverPercentage  / 2.0f));
		lerpedValue.w = 1.0f;
		TODColor = lerpedValue;
		TODColor.w = 1.0f -CloudCoverPercentage;
		

		if(m_lightMapCamera != null)
		{
			
			m_lightMapCamera.backgroundColor = lerpedValue;
		}
		else
		{
			Debug.Log("Camera not found");	
		}
		//Shader.SetGlobalVector("_TOD", lerpedValue);
		
	}
	
	private void AdvanceFrame()
	{
		m_currentFrame = m_frames[m_currentFrameIndex];
		m_nextFrame = m_frames[(m_currentFrameIndex + 1) % m_frames.Count];
		m_currentFrameIndex++;
		m_currentFrameIndex = m_currentFrameIndex % m_frames.Count;
	}
	
	private void OnGUI()
	{
		GUI.Label(new Rect(20, 200, 200, 200), (ActiveTime / CycleTime).ToString());
	}
	
	public List<TODKeyFrame> Frames
	{
		get { return m_frames; }
		set { m_frames = value; }
	}
	
	public float AdjustedTime
	{
		get { return ActiveTime / CycleTime; }
		set { ActiveTime = (value * CycleTime); }
	}
	
	private int					m_currentFrameIndex;
	private TODKeyFrame 		m_currentFrame;
	private TODKeyFrame 		m_nextFrame;
	
	[SerializeField]
	private List<TODKeyFrame> 	m_frames;
	private Camera 				m_lightMapCamera = null;
}

[Serializable]
public class TODKeyFrame : IComparable<TODKeyFrame>
{
	[SerializeField]
	public float FrameTime;
	
	[SerializeField]
	public Vector4 FrameColor;
	
	[SerializeField] 
	public float CloudCoverMultiplier;
	
	public int CompareTo(TODKeyFrame other)
	{
		if(other == this)
		{
			return 0;	
		}
		return other.FrameTime < FrameTime ? 1 : -1;
	}
}
