using UnityEngine;
using System.Collections;

public class DoorSwingTest : MonoBehaviour 
{
	public bool pause = false;
	public float lerpSpeed = 0.02f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(false && m_lerpProgress >= 1.0f)
		{
			m_lerpProgress = 0.0f;
			m_lastAngle = m_targetAngle;
			m_targetAngle = -(Random.value * 90.0f);
		}
		
		if(pause)
		{
			m_lerpProgress += lerpSpeed;
		}
		transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_lerpProgress));
	
	}
	
	private  float m_lastAngle = 0.0f;
	private float m_targetAngle = 0.0f;
	private float m_lerpProgress = 0.0f;
}
