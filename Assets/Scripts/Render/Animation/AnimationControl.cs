using UnityEngine;
using System.Collections;

public class AnimationControl : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
		m_anim = GetComponent<Animator>();
	}


	// Update is called once per frame
	void Update () {

		test++;
		m_anim.SetFloat("cold", cold);
		m_anim.SetFloat("speed", speed);
	}

	void OnGUI()
	{
		//cold = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 100), cold, 0.0f, 1.0f);
		//speed = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 100, Screen.height - 500, 200, 100), speed, 0.0f, 1.0f);
	}

	public void DoThing()
	{

	}

	private int test = 0;
	public float speed = 0.0f;
	public float cold = 0.0f;
	private Animator m_anim;
}
