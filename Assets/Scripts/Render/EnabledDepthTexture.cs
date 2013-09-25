///////////////////////////////////////////////////////////
// 
// EnabledDepthTexture.cs
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

public class EnabledDepthTexture : MonoBehaviour 
{

	void Start ()
	{
		camera.depthTextureMode = DepthTextureMode.Depth;
	}

}
