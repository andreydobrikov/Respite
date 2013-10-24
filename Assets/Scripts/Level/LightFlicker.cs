///////////////////////////////////////////////////////////
// 
// LightFlicker.cs
//
// What it does: Flicker's a MeshRenderer's Color
//
// Notes: I could split this into a couple of distinct flicker scripts, 
//		  but who can be arsed with this
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class LightFlicker : MonoBehaviour 
{
	public enum FlickerType
	{
		Bulb,
		Fire
	}
	
	[ShaderPropertyNameAttribute(ShaderPropertyNameAttribute.PropertyType.Color)]
	public string ColorParameter = string.Empty;
	
	public FlickerType FlickerStyle = FlickerType.Fire;
	
	public Color FlickerTargetColour;
	
	public float PhaseRange = 4.0f;
	
	public Color TargetColour;
	
	// Use this for initialization
	void Start () 
	{
		m_initialColour = renderer.sharedMaterial.GetColor(ColorParameter);
		m_lastColour = m_initialColour;
		m_phaseTime = Random.value * PhaseRange;
		m_phaseProgress = 0.0f;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		switch(FlickerStyle)
		{
			case FlickerType.Bulb:
			{
				if(Random.value > 0.99f)
				{
					m_dimCount = Random.Range(1, 20);	
				}
				
				if(m_dimCount > 0)
				{
					m_dimCount--;
					renderer.material.SetColor(ColorParameter, FlickerTargetColour);
				}
				else
				{
					renderer.material.SetColor(ColorParameter, m_initialColour);
				}
				break;
			}
			
			case FlickerType.Fire:
			{
				m_phaseProgress += Time.deltaTime;
			
				if(m_phaseProgress > m_phaseTime)
				{
					m_lastColour = m_targetColour;
					m_targetColour = Vector4.Lerp(m_initialColour, FlickerTargetColour, Random.value);
					TargetColour = m_targetColour;
					m_phaseTime = 0.5f + Random.value * PhaseRange;
					m_phaseProgress = 0.0f;
				}
			
			float progress = 1.0f - ((m_phaseTime - m_phaseProgress) / m_phaseTime);
				renderer.material.SetColor(ColorParameter, Vector4.Lerp(m_lastColour, m_targetColour, progress));
			
				break;
			}
		}
		
	}
	
	private Vector4 m_initialColour;
	
	// Flicker vars
	private int m_dimCount 				= 0;
	
	// Fire vars
	private Color m_targetColour;
	private Color m_lastColour;
	private float m_phaseTime;
	private float m_phaseProgress;
}
