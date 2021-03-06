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
	public float ActiveTime 			= 0.0f;
	public bool PauseUpdate 			= false;
	public float CloudCoverPercentage 	= 0.0f;
	public Vector4 TODColor 			= Vector4.one;
	public float TODColorMagnitude		= 1.0f;
	
	void OnEnable()
	{
		Reset ();
	}
	
	void Start()
	{
		m_cycleTime = GameFlow.Instance.GameDuration;
		Reset ();

		PauseUpdate = false;
	}
	
	private void Reset()
	{
		GameObject cameraObject = GameObject.FindGameObjectWithTag("LightMapCamera");
		
		if(cameraObject != null)
		{
			m_lightMapCamera = cameraObject.GetComponent<Camera>();
		}
		
		ActiveTime 			= 0.0f;
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
	
	void Update () 
	{
		if(GameTime.Instance.Paused)
		{
			return;	
		}
		
		if(!PauseUpdate)
		{
			ActiveTime += Time.deltaTime; 
		}
		
		UpdateTime(false);
	}
	
	public void UpdateTime(bool recalculateFrame)
	{
		float adjustedTime = ActiveTime / m_cycleTime;
		
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
		
		if(ActiveTime > m_cycleTime)
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
		TODColorMagnitude = ((Vector3)TODColor).magnitude; // The magnitude of the TOD color is used all over the shop, so just grab it once here.

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
			
			m_updateString = "Time Of Day " + (m_showFoldout ? "" : (time.Hours.ToString("00") + ":" + time.Minutes.ToString("00")));
			
			m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, m_updateString);
			
			
			if(m_showFoldout)
			{
				GUILayout.BeginVertical((GUIStyle)("Box"));
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Time", GUILayout.Width(30));
				AdjustedTime = GUILayout.HorizontalSlider(AdjustedTime, 0.0f, 1.0f);
				GUILayout.Label((time.Hours.ToString("00") + ":" + time.Minutes.ToString("00")), GUILayout.Width(40));
				
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
		get { return ActiveTime / m_cycleTime; }
		set { ActiveTime = (value * m_cycleTime); }
	}
	
	void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("time", ActiveTime.ToString()));
	}
	
	void SaveDeserialise(List<SavePair> pairs)
	{
		float activeTime = 0.0f;
		
		foreach(var pair in pairs)
		{
			if(pair.id == "time") { float.TryParse(pair.value, out activeTime); }
		}
		
		ActiveTime = activeTime;
	}
	
	private int					m_currentFrameIndex;
	private TODKeyFrame 		m_currentFrame;
	private TODKeyFrame 		m_nextFrame;

	[SerializeField]
	private List<TODKeyFrame> 	m_frames;
	private Camera 				m_lightMapCamera = null;
	
	private string 				m_updateString = string.Empty;
	private float				m_cycleTime = 10.0f;
	
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
