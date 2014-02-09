/// <summary>
/// Player.cs
/// 
/// 
/// 
/// </summary>


using UnityEngine;
using System.Collections.Generic;

public class Player : Entity, ISerialisable
{
	public float CooldownRate = 0.0001f;
	
    public override float GetVisibilityRadius()
    {
        return 0.6f;
    }

	protected override void EntityStart() 
	{
        m_entityName = "Player";

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
	
	public void SaveSerialise(List<SavePair> pairs)
	{
		pairs.Add(new SavePair("position_x", rigidbody.position.x.ToString()));
		pairs.Add(new SavePair("position_y", rigidbody.position.y.ToString()));
		pairs.Add(new SavePair("position_z", rigidbody.position.z.ToString()));
		
		pairs.Add(new SavePair("rotation_x", rigidbody.rotation.x.ToString()));
		pairs.Add(new SavePair("rotation_y", rigidbody.rotation.y.ToString()));
		pairs.Add(new SavePair("rotation_z", rigidbody.rotation.z.ToString()));
		pairs.Add(new SavePair("rotation_w", rigidbody.rotation.w.ToString()));
	}
	
	public void SaveDeserialise(List<SavePair> pairs)
	{
		Vector3 position 	= Vector3.one;
		Quaternion rotation = Quaternion.identity;
		
		foreach(var pair in pairs)
		{
			if(pair.id == "position_x") { float.TryParse(pair.value, out position.x); }
			if(pair.id == "position_y") { float.TryParse(pair.value, out position.y); }
			if(pair.id == "position_z") { float.TryParse(pair.value, out position.z); }
			
			if(pair.id == "rotation_x") { float.TryParse(pair.value, out rotation.x); }
			if(pair.id == "rotation_y") { float.TryParse(pair.value, out rotation.y); }
			if(pair.id == "rotation_z") { float.TryParse(pair.value, out rotation.z); }
			if(pair.id == "rotation_w") { float.TryParse(pair.value, out rotation.w); }
		}
		
		rigidbody.position = position;
		rigidbody.rotation = rotation;
	}
	
	public float Warmth { get; set; }
	public float Energy { get; set; }
}

