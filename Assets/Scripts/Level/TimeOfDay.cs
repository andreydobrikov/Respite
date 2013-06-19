using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour 
{
	public float CycleTime = 1000.0f;
	public float ActiveTime = 0.0f;
	public float StartTime = 0.0f;
	public bool PauseUpdate = false;
	public float CloudCoverPercentage = 0.0f;
	
	void OnEnable()
	{
		Reset ();
	}
	
	void Start()
	{
		Reset ();
	}
	
	private void Reset()
	{
		GameObject cameraObject = GameObject.FindGameObjectWithTag("LightMapCamera");
		if(cameraObject != null)
		{
			m_lightMapCamera = cameraObject.GetComponent<Camera>();
			
			int pixelWidth = (int)m_lightMapCamera.pixelWidth / 4;
			int pixelHeight = (int)m_lightMapCamera.pixelHeight / 4;
			
			Debug.Log("Width: " + pixelWidth);
			
			m_lightMapCamera.targetTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			m_lightMapCamera.targetTexture.isPowerOfTwo = false;
			Camera.mainCamera.GetComponent<LightMapEffect>().lightMapTexture = m_lightMapCamera.targetTexture;
			
		}
		ActiveTime 			= StartTime;
		m_currentFrameIndex = 0;
		
		TODKeyFrame frame1 = new TODKeyFrame();
		TODKeyFrame frame2 = new TODKeyFrame();
		TODKeyFrame frame3 = new TODKeyFrame();
		TODKeyFrame frame4 = new TODKeyFrame();
		TODKeyFrame frame5 = new TODKeyFrame();
		
		frame1.FrameColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		frame1.FrameTime = 0.2f;
		
		frame2.FrameColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		frame2.FrameTime = 0.7f;
		
		frame3.FrameColor = new Vector4(0.1f, 0.1f, 0.2f, 1.0f);
		frame3.FrameTime = 0.1f;
		
		frame4.FrameColor = new Vector4(0.1f, 0.1f, 0.2f, 1.0f);
		frame4.FrameTime = 0.85f;
		
		frame5.FrameColor = new Vector4(0.95f, 0.75f, 0.5f, 1.0f);
		frame5.FrameTime = 0.78f;
		
		m_frames.Clear();
		
		m_frames.Add(frame1);
		m_frames.Add(frame2);
		m_frames.Add(frame3);
		m_frames.Add(frame4);
		m_frames.Add(frame5);
		m_frames.Sort();
		
		AdvanceFrame();
		UpdateTime(true);
	}
	
	// Update is called once per frame
	Vector4 SunColor = new Vector4();
	
	void FixedUpdate () 
	{
		if(!PauseUpdate)
		{
			ActiveTime += Time.deltaTime;
		}
		
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
			Debug.Log("Advance...");
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
		
		lerpedValue *= (1.0f - (CloudCoverPercentage  / 3.0f));

		if(m_lightMapCamera != null)
		{
			lerpedValue.w = 1.0f;
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
		GUI.Label(new Rect(200, 200, 200, 200), (ActiveTime / CycleTime).ToString());
	}
	
	public List<TODKeyFrame> Frames
	{
		get { return m_frames; }
		set { m_frames = value; }
	}
	
	public float AdjustedTime
	{
		get { return ActiveTime / CycleTime; }
	}
	
	private int					m_currentFrameIndex;
	private TODKeyFrame 		m_currentFrame;
	private TODKeyFrame 		m_nextFrame;
	private List<TODKeyFrame> 	m_frames = new List<TODKeyFrame>();
	private Camera 				m_lightMapCamera = null;
}

public class TODKeyFrame : IComparable<TODKeyFrame>
{
	public float FrameTime;
	public Vector4 FrameColor;
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
