///////////////////////////////////////////////////////////
// 
// GlobalTextureScroll.cs
//
// What it does: Terrible name. Basically a TextureScroll that's rotationally invariant.
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class GlobalTextureScroll : TextureScroll
{
	public float UScroll = 0.0f;
	public float VScroll = 0.0f;

	void Start ()
	{
		Vector3 localVec = new Vector3(UScroll, 0.0f, VScroll);

		Vector3 newVec = Quaternion.Inverse(transform.rotation) * localVec;
		UScrollRate = newVec.x;
		VScrollRate = newVec.z;

		m_renderer = GetComponent<MeshRenderer>();
	}

}
