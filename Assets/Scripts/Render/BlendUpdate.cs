///////////////////////////////////////////////////////////
// 
// BlendUpdate.cs
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

public class BlendUpdate : MonoBehaviour 
{
	public float Rate = 1.0f;
	
	void Start (){

	}
	
	float temp = 0.0f;
	float delta = 1.0f;
	void Update ()
	{
		temp += (Time.deltaTime * delta) * Rate;
		if(temp > 1.0f)
		{
			delta = -1.0f;
		}
		if(temp < 0.0f)
		{
			delta = 1.0f;	
		}
		Shader.SetGlobalFloat("BlendLerp", temp);
	}
}
