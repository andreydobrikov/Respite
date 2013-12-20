using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roof : MonoBehaviour {

	public float fadeRate = 0.1f;
	public List<GameObject> ObjectsToFade = new List<GameObject>();
	public List<RoofTrigger> EntryTriggers = new List<RoofTrigger>();

	// Use this for initialization
	void Start () 
	{
		foreach(var trigger in EntryTriggers)
		{
			trigger.SetRoofTrigger(this);
		}

		foreach(var currentFadeObject in ObjectsToFade)
		{
			if(currentFadeObject.renderer != null)
			{
				Material material = currentFadeObject.renderer.material;
				if(material != null)
				{
					FadeMaterial newFadeMaterial = new FadeMaterial();
					newFadeMaterial.material = material;
					newFadeMaterial.initialAlpha = material.color.a;

					m_materialsToFade.Add(newFadeMaterial);
				}
			}
		}
	}

	public void Update()
	{
		if(m_fadeDown && m_alphaLerp > 0.0f)
		{
			m_alphaLerp -= GameTime.DeltaTime * fadeRate;

			foreach(var mat in m_materialsToFade)
			{
				float newAlpha = Mathf.Lerp(0.0f, mat.initialAlpha, m_alphaLerp);
				Color oldColor = mat.material.color;
				oldColor.a = newAlpha;

				mat.material.color = oldColor;
			}
		}
		else if(!m_fadeDown && m_alphaLerp < 1.0f)
		{
			m_alphaLerp += GameTime.DeltaTime * fadeRate;
			
			foreach(var mat in m_materialsToFade)
			{
				float newAlpha = Mathf.Lerp(0.0f, mat.initialAlpha, m_alphaLerp);
				Color oldColor = mat.material.color;
				oldColor.a = newAlpha;
				
				mat.material.color = oldColor;
			}
		}
	}
	
	public void TriggerEntered()
	{
		m_fadeDown = true;
		Debug.Log("Fading roof");
	}

	public void TriggerExited()
	{
		m_fadeDown = false;
	}

	// Use a direct list of materials internally to keep things a bit snappier
	private List<FadeMaterial> m_materialsToFade = new List<FadeMaterial>();

	private struct FadeMaterial
	{
		public Material material;
		public float initialAlpha;
	}

	private bool m_fadeDown = false;
	private float m_alphaLerp = 1.0f;
}
