#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour
{
	public GameObject OverlayObject 	= null;
	public GameObject ParticleObject 	= null;
	
	public void Start()
	{
		m_timeOfDay = GameObject.FindObjectOfType(typeof(TimeOfDay)) as TimeOfDay;
		
		m_timeOfDay.CloudCoverPercentage = m_cloudCover;
		
		
		if(OverlayObject != null)
		{
			m_overlayRenderer = OverlayObject.GetComponent<MeshRenderer>();
			
			if(m_overlayRenderer != null)
			{
				m_overlayAlpha = m_overlayRenderer.sharedMaterial.color.a;
			}
		}
		
		if(ParticleObject != null)
		{
			m_snowParticles = ParticleObject.GetComponent<ParticleSystem>();
		}
	}

	private void OnGUI()
	{
		
#if UNITY_EDITOR
		if(Application.isPlaying)
		{
			GUILayout.BeginArea(new Rect(240, 10, 300, m_showFoldout ? 300 : 30));
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			m_showFoldout = EditorGUILayout.Foldout(m_showFoldout, "Weather");
			
			
			if(m_showFoldout)
			{
				
				GUILayout.BeginVertical((GUIStyle)("Box"));
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Storm Intensity", GUILayout.Width(120));
				
				float stormIntensity = GUILayout.HorizontalSlider(m_stormIntensity, 0.0f, 1.0f);
				
				if(stormIntensity != m_stormIntensity)
				{
					CloudCover = stormIntensity;
					SetOverlayAlpha(stormIntensity);
					SetParticleIntensity(stormIntensity);
					
					m_stormIntensity = stormIntensity;	
				}
				
				GUILayout.Label(m_stormIntensity.ToString("0.0"));
				
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
				
				GUILayout.BeginVertical((GUIStyle)("Box"));
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Cloud Cover", GUILayout.Width(120));
				CloudCover = GUILayout.HorizontalSlider(CloudCover, 0.0f, 1.0f);
				GUILayout.Label(CloudCover.ToString("0.0"));
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Overlay Alpha", GUILayout.Width(120));
				float newAlpha = GUILayout.HorizontalSlider(m_overlayAlpha, 0.0f, 1.0f);
				if(newAlpha != m_overlayAlpha && m_overlayRenderer != null)
				{
					SetOverlayAlpha(newAlpha);
				}
				
				GUILayout.Label(m_overlayAlpha.ToString("0.0"));
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label("Particle Intensity", GUILayout.Width(120));
				float newParticleIntensity = GUILayout.HorizontalSlider(m_particleIntensity, 0.0f, 1.0f);
				if(newParticleIntensity != m_particleIntensity)
				{
					SetParticleIntensity(newParticleIntensity);
				}
				
				GUILayout.Label(m_particleIntensity.ToString("0.0"));
				
				GUILayout.EndHorizontal();
				
				m_showParticlesFoldout = EditorGUILayout.Foldout(m_showParticlesFoldout, "Particle Settings");
				
				GUILayout.EndVertical();
				
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
		}
		
#endif
	}
	
	public float CloudCover
	{
		get { return m_cloudCover; }
		set
		{
			m_cloudCover = value;
			m_timeOfDay.CloudCoverPercentage = m_cloudCover;
		}
	}
	
	private void SetOverlayAlpha(float alpha)
	{
		if(m_overlayRenderer != null)
		{
			Vector4 color = m_overlayRenderer.sharedMaterial.color;
			color.w = alpha;
			m_overlayRenderer.sharedMaterial.color = color;
			m_overlayAlpha = alpha;
		}
	}
	
	private void SetParticleIntensity(float intensity)
	{
		if(m_snowParticles != null)
		{
			m_snowParticles.emissionRate 	= m_particlesMax * intensity;	
			m_snowParticles.startSpeed 		= m_particlesMaxStartSpeed * intensity;
			m_snowParticles.startLifetime	= m_particlesMinLifetime + ((m_particlesMaxLifetime - m_particlesMinLifetime) * (1.0f - intensity));
			m_snowParticles.startSize		= m_particlesMinStartSize + ((m_particlesMaxStartSize - m_particlesMinStartSize) * intensity);
		}
		m_particleIntensity = intensity;
	}
	
	private TimeOfDay m_timeOfDay = null;
	
	private ParticleSystem m_snowParticles 	= null;
	private MeshRenderer m_overlayRenderer 	= null;
	private float m_stormIntensity 			= 0.0f;
	private float m_particleIntensity 		= 0.0f;
	private float m_overlayAlpha 			= 0.0f;
	private float m_cloudCover				= 0.0f;
	private bool m_showFoldout 				= false;
	private bool m_showParticlesFoldout 	= false;
	
	private const int m_particlesMax 				= 1500;
	private const float m_particlesMaxStartSpeed 	= 10.0f;
	private const float m_particlesMaxLifetime		= 10.0f;
	private const float m_particlesMinLifetime		= 1.0f;
	private const float m_particlesMaxStartSize		= 0.15f;
	private const float m_particlesMinStartSize		= 0.01f;
}
