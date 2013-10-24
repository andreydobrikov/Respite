///////////////////////////////////////////////////////////
// 
// AIBehaviourWatchForPlayer.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public class AIBehaviourWatchForPlayer : AIBehaviour 
{
	public AIBehaviourWatchForPlayer()
	{
		m_name = "Watch For Player";	
		m_supportTransitions = true;
	}
	
	public override void Start() 
	{
		m_player = GameObject.FindGameObjectWithTag("Player");
		m_spotProgress = 0.0f;
	}
	
	public override bool Update() 
	{
		if(Time.timeScale == 0.0f)
		{
			return false;	
		}
		
		m_spotProgress -= (m_spotDecay * Time.deltaTime);
		m_spotProgress = Mathf.Max(m_spotProgress, 0.0f);
		
		// TODO: Oh dear. Sort this parent name business
		if(Parent.Parent.PlayerInPerceptionRange)
		{
			Quaternion orientation = GetObject().transform.rotation;
			float aiAngle = orientation.eulerAngles.y;
			
			Vector3 position = GetObject().transform.position;
			Vector3 direction = m_player.transform.position - position;
			
			float distanceToPlayer = direction.magnitude;
			float perceptionRange = Parent.Parent.PerceptionRange;
			
			float angle = Mathf.Atan2(direction.x, direction.z);
			
			Vector3 startRay = Quaternion.Euler(0.0f, (aiAngle - m_viewAngle / 2.0f ) , 0.0f) * Vector3.forward;
			Vector3 endRay = Quaternion.Euler(0.0f, (aiAngle + m_viewAngle / 2.0f ), 0.0f) * Vector3.forward;
			
			startRay *= perceptionRange;
			endRay *= perceptionRange;
				
			Debug.DrawRay(position, startRay, Color.blue);
			Debug.DrawRay(position, endRay, Color.magenta);
			
			
			const int sweepValues = 5;
			
			// TODO: 0.3f should be player collider radius
			// TODO: Aw, man. Trig functions again
			
			float targetColliderOffset = Mathf.Atan(0.3f / distanceToPlayer);
			
			float sweepStart = angle - targetColliderOffset;
			float sweepDelta = (targetColliderOffset * 2.0f) / (float)sweepValues;
			
			RaycastHit hitInfo;
			
			float distanceMultiplier = 1.0f - Mathf.Sin((distanceToPlayer/ perceptionRange) * (Mathf.PI / 2.0f));
			for(int i = 0; i < sweepValues; ++i)
			{
				float currentAngle = (sweepStart + (sweepDelta * i)) * Mathf.Rad2Deg;	
				Vector3 rayDirection = Quaternion.Euler(0.0f, currentAngle, 0.0f) * Vector3.forward;
				
				float relativeAngle = Mathf.Acos(Vector3.Dot(rayDirection, orientation * Vector3.forward)) * Mathf.Rad2Deg;
				
				if(relativeAngle < m_viewAngle / 2.0f)
				{
					Debug.DrawRay(position, rayDirection * distanceToPlayer, Color.red);
					
					if(!Physics.Raycast(position, rayDirection, out hitInfo, distanceToPlayer, 1 << LayerMask.NameToLayer("LevelGeo")))
					{
						m_spotProgress += ((m_spotRate * Time.deltaTime) * distanceMultiplier);
						
					} 
				}
			}
		}
		
		if(m_progressBar != null)
		{
			m_progressBar.progress = m_spotProgress;	
		}
		
		if(m_spotProgress >= 1.0f)
		{
			if(m_endGameOnSight)
			{
				GameFlow.Instance.GameOver();
			}
			return true;	
		}
		
		return false;
	}
	
	public override void End() { }
	
#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		m_viewAngle 		= EditorGUILayout.Slider("View Angle", m_viewAngle, 1.0f, 360.0f);
		m_endGameOnSight 	= EditorGUILayout.Toggle("End Game", m_endGameOnSight);
		m_spotDecay 		= EditorGUILayout.FloatField("Spot Decay", m_spotDecay);
		m_spotRate 			= EditorGUILayout.FloatField("Spot Rate", m_spotRate);
		m_progressBar 		= EditorGUILayout.ObjectField("Progress Bar", m_progressBar, typeof(ProgressBar), true) as ProgressBar;
		
		GUI.enabled = false;
		EditorGUILayout.FloatField("Spot Progress", m_spotProgress);
		GUI.enabled = true;
	}
#endif
	
	private GameObject m_player 		= null;
	private float m_spotProgress 		= 0.0f;
	
	[SerializeField]
	private ProgressBar m_progressBar 	= null;
	
	[SerializeField]
	private float m_viewAngle = 45.0f;
	
	[SerializeField]
	private bool m_endGameOnSight = false;
	
	[SerializeField]
	private float m_spotDecay = 0.3f;
	
	[SerializeField]
	private float m_spotRate = 0.5f;
}
