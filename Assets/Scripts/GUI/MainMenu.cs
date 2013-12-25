using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public int MenuWidth = 300;

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect((Screen.width / 2) - (MenuWidth / 2), Screen.height / 2 - 300, MenuWidth, 60), (GUIStyle)("Box"));

		GUILayout.BeginVertical();

		GUILayout.Label("Respite");

		if(GUILayout.Button("Start") || Input.GetButtonDown("select"))
		{
			m_advance = true;
		}

		GUILayout.EndVertical();

		GUILayout.EndArea();
	}

	void Start()
	{
		Time.timeScale = 1.0f;

		m_advance = false;
		m_transition = false;
		m_fadeUp = false;

		m_fade = FindObjectOfType<CameraFade>();
		m_fade.StartFade(Color.black, 0.0f, StartFadeComplete);

	}

	void Update()
	{
		if(Input.GetButtonDown("select"))
		{
			m_advance = true;
		}

		if(m_fadeUp)
		{
			m_fade.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 0.8f, null);
			m_fadeUp = false;
		}

		if(m_advance)
		{
			m_advance = false;
			if(m_fade != null)
			{
				m_fade.StartFade(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f, FadeComplete);
			}
			else
			{
				m_transition = true;
			}
		}

		if(m_transition)
		{
			Application.LoadLevel("respite_main");
		}
	}

	void FadeComplete()
	{
		m_transition = true;
	}

	void StartFadeComplete()
	{
		m_fadeUp = true;
	}


	CameraFade m_fade = null;
	bool m_advance = false;
	bool m_transition = false;
	bool m_fadeUp = false;
}
