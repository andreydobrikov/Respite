#if UNITY_EDITOR
	using UnityEditor;
#endif

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
		Shader.SetGlobalVector("_TOD", lerpedValue);
		
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
		
#if UNITY_EDITOR
		if(Application.isPlaying)
		{
			GUILayout.BeginArea(new Rect(20, 10, 200, m_showFoldout ? 300 : 30));
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			float minutes = 1440.0f * AdjustedTime;
			TimeSpan time = TimeSpan.FromMinutes(minutes);
			
			m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Time Of Day " + (m_showFoldout ? "" : (time.Hours.ToString("00") + ":" + time.Minutes.ToString("00"))));
			
			
			if(m_showFoldout)
			{
				GUILayout.BeginVertical((GUIStyle)("Box"));
				PauseUpdate = GUILayout.Toggle(PauseUpdate, "Pause");
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Time", GUILayout.Width(30));
				AdjustedTime = GUILayout.HorizontalSlider(AdjustedTime, 0.0f, 1.0f);
				GUILayout.Label((time.Hours.ToString("00") + ":" + time.Minutes.ToString("00")), GUILayout.Width(40));
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Cycle Time", GUILayout.Width(40));
				string newCycleTimeString = GUILayout.TextField(CycleTime.ToString("0.00"));
				
				float newCycleTime = CycleTime;
				if(float.TryParse(newCycleTimeString, out newCycleTime) && newCycleTime > 0.0f)
				{
					CycleTime = newCycleTime;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
		}
		
#endif
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
	
#if UNITY_EDITOR
	private bool m_showFoldout = false;
#endif
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
