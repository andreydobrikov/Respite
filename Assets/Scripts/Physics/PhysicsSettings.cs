using UnityEngine;
using System.Collections;


public class PhysicsSettings : MonoBehaviour 
{
	
	private static PhysicsSettings m_instance = null;	
	
	public static PhysicsSettings Instance
	{
		get
		{
			if(m_instance == null)
			{
				Debug.LogError("PhysicsSettings accessed prior to instantiation");
			}
			
			return m_instance;
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		m_instance = this;
	}
	
	private PhysicsSettings()
	{
		m_instance = this;
	}
	
	public float Gravity 			= -0.015f;
	public float TerminalVelocity 	= 1.0f;
	
	
}
