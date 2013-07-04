using UnityEngine;
using System.Text;
using System.Collections.Generic;

[AddComponentMenu("Custom/GUI/Interaction Menu")]
public class InteractionMenu : MonoBehaviour 
{
	public Font font;
	public GUISkin skin;
	
	public InteractionMenu()
	{
		
	}
	
	void Start()
	{
		m_style = new GUIStyle();
		m_style.font = font;
		m_style.fontSize = 20;
	}
	
	void OnTriggerEnter(Collider other)
	{
		InteractiveObject interactiveObject = other.GetComponent<InteractiveObject>();
		
		if(interactiveObject != null)
		{
			m_objectsInRange.Add(interactiveObject);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		InteractiveObject interactiveObject = other.GetComponent<InteractiveObject>();
		
		if(interactiveObject != null)
		{
			m_objectsInRange.Remove(interactiveObject);	
		}
	}
	
	void OnGUI()
	{
		if(Event.current.type == EventType.KeyDown)
		{
			if(Input.GetKeyDown(KeyCode.Tab))
			{
				m_currentTab = ++m_currentTab % m_objectsInRange.Count;
			}
			
		}
		
		if(m_objectsInRange.Count > 0)
		{
			if(m_currentTab >= m_objectsInRange.Count)
			{
				m_currentTab = m_objectsInRange.Count - 1;	
			}
			
			GUI.skin = skin;
			
			
			Vector2 centrePoint = new Vector2(Screen.width / 2, Screen.height / 2);
			
			if(m_objectsInRange.Count > 1)
			{
				for(int i = 0; i < m_objectsInRange.Count; i++)
				{
					const float dotWidth = 10.0f;
					
					if(m_currentTab == i)
					{
						GUI.Label(new Rect(centrePoint.x - 50.0f + (i * 10.0f), centrePoint.y - 50.0f, 150.0f, 50.0f), ".", (GUIStyle)("Tab"));
					}
					else
					{
						GUI.Label(new Rect(centrePoint.x - 50.0f + (i * 10.0f), centrePoint.y - 50.0f, 150.0f, 50.0f), ".");
					}
					
				}
			}
			
			List<Interaction> interactions = m_objectsInRange[m_currentTab].GetInteractions();
			if(interactions.Count > 3)
			{
				Debug.Log("Too many interactions");	
			}
			
			GUI.enabled = interactions[0].Enabled;
			if(GUI.Button(new Rect(centrePoint.x + 20.0f, centrePoint.y - 15.0f, 100.0f, 30.0f), interactions[0].Name))
			{
				interactions[0].Callback(interactions[0]);	
			}
			
			if(interactions.Count > 1)
			{
				GUI.enabled = interactions[1].Enabled;
				GUIStyle current = GUI.skin.GetStyle("Button");
				Vector2 size = current.CalcSize(new GUIContent(interactions[1].Name));
				
				if(GUI.Button(new Rect(centrePoint.x - 40.0f - size.x, centrePoint.y - 15.0f, size.x, 30.0f), interactions[1].Name))
				{
					interactions[1].Callback(interactions[1]);	
				}
			}
			
			//GUILayout.EndArea();
		}
	}
	
	List<InteractiveObject> m_objectsInRange = new List<InteractiveObject>();
	
	private GUIStyle m_style;
	private int m_currentTab = 0;
}
