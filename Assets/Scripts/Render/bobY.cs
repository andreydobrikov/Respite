///////////////////////////////////////////////////////////
// 
// bobAI.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class bobY : MonoBehaviour 
{
	public float maxY = 0.0f;
	public float minY = -5.0f;
	public float animTime = 2.0f;
	public float texAnimRate = 0.0005f;
	
	void Start ()
	{
		m_scroll = GetComponent<TextureScroll>();
	}

	void Update ()
	{
		lerpProgress += (lerpDirection * (Time.deltaTime / animTime));
		
		if(lerpProgress < 0.0f && lerpDirection < 0.0f)
		{
			lerpDirection = -lerpDirection;	
			m_scroll.UScrollRate = -texAnimRate;
		}
		else if(lerpProgress > 1.0f && lerpDirection > 0.0f)
		{
			lerpDirection = -lerpDirection;	
			m_scroll.UScrollRate = texAnimRate;
		}
		
		float progress = Mathf.Sin((Mathf.PI / 2.0f) + (lerpProgress * Mathf.PI));
		
		progress += 1.0f;
		progress /= 2.0f;
		float y = Mathf.Lerp(minY, maxY, progress);
		
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}
	
	private float lerpProgress = 0.0f;
	private float lerpDirection = -1.0f;
	private TextureScroll m_scroll;
}
