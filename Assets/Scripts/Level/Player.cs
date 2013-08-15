/// <summary>
/// Player.cs
/// 
/// 
/// 
/// </summary>


using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
	public float CooldownRate = 0.0001f;
	
	void Start () 
	{
		Warmth = 1.0f;
		Energy = 1.0f;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Warmth -= CooldownRate;
		Warmth = Mathf.Max(0.0f, Warmth);
	}
	
	void OnGUI()
	{
		/*
		GUILayout.BeginArea(new Rect(Screen.width - 110.0f, 10.0f, 100.0f, 300.0f));
		
		GUILayout.Label("Player State");
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		GUILayout.Label("Warmth: " + Warmth.ToString("0.00"));
		GUILayout.Label("Energy: " + Energy.ToString("0.00"));
		
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
		*/
	}
	
	void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("position_x", rigidbody.position.x.ToString()));
		pairs.Add(new SavePair("position_y", rigidbody.position.y.ToString()));
		pairs.Add(new SavePair("position_z", rigidbody.position.z.ToString()));
	}
	
	void SaveDeserialise(List<SavePair> pairs)
	{
		Vector3 position = Vector3.one;
		
		foreach(var pair in pairs)
		{
			if(pair.id == "position_x") { float.TryParse(pair.value, out position.x); }
			if(pair.id == "position_y") { float.TryParse(pair.value, out position.y); }
			if(pair.id == "position_z") { float.TryParse(pair.value, out position.z); }
		}
		rigidbody.position = position;
	}
	
	public float Warmth { get; set; }
	public float Energy { get; set; }
}

