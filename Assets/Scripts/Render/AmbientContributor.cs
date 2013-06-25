using UnityEngine;
using System.Collections;

[AddComponentMenu("Custom/Rendering/AmbientContributor")]
public class AmbientContributor : MonoBehaviour 
{
	public float contribution = 0.5f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Vector4 GetAmbientContribution()
	{
		return renderer.material.color * contribution;	
	}
}
