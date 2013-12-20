using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DisableMeshInEditor : MonoBehaviour 
{
	void Start()
	{
		//renderer.enabled = true;
	}
	
	void OnEnable()
	{
#if UNITY_EDITOR

		if(Application.isEditor && !Application.isPlaying)
		{
			
			renderer.enabled = false;
		}
		else
		{
			renderer.enabled = true;
		}
		
#else
		renderer.enabled = true;
#endif
		
	}
}
