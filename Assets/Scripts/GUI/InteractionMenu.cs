using UnityEngine;
using System.Text;
using System.Collections.Generic;

[AddComponentMenu("Custom/GUI/Interaction Menu")]
public class InteractionMenu : MonoBehaviour 
{
	public Font font;
	public GUISkin skin;
	public float InspectionAngleMax = 60.0f;
	public LayerMask collisionLayer = 0;
	public bool m_renderDebug		= false;
	
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
		if(m_objectsInView.Count > 0)
		{
			if(Event.current.type == EventType.KeyDown)
			{
				if(Input.GetKeyDown(KeyCode.Tab))
				{
					m_currentTab = ++m_currentTab % m_objectsInView.Count;
				}
			}
		
			GUI.skin = skin;
			
			Vector2 centrePoint = new Vector2(Screen.width / 2, Screen.height / 2);
			
			if(m_objectsInView.Count > 1)
			{
				for(int i = 0; i < m_objectsInView.Count; i++)
				{
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
			
			
			if(m_currentTab > m_objectsInView.Count)
			{
				m_currentTab = Mathf.Max(m_objectsInView.Count, 0);	
			}
			
			if(m_objectsInView.Count > 0)
			{
				List<Interaction> interactions = m_objectsInView[0].GetInteractions();
				if(interactions.Count > 3)
				{
					Debug.Log("Too many interactions");	
				}
				
				// No object should ever have zero interactions, but better to handle it as the editor can sometimes cause trouble
				// When reloading scripts.
				if(interactions.Count > 0)
				{
					GUI.enabled = interactions[0].Enabled;
					if(GUI.Button(new Rect(centrePoint.x + 20.0f, centrePoint.y - 15.0f, 100.0f, 30.0f), interactions[0].Name))
					{
						interactions[0].Callback(interactions[0]);	
					}
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
			}
		}
	}
	
	void Update()
	{

		if(m_objectsInView.Count > 0)
		{
			if(m_currentTab >= m_objectsInRange.Count)
			{
				m_currentTab = m_objectsInRange.Count - 1;	
			}
			
			if(m_currentTab >= m_objectsInRange.Count)
			{
				m_currentTab = m_objectsInRange.Count - 1;	
			}
		
			List<Interaction> interactions = m_objectsInView[0].GetInteractions();	
		
			if(Input.GetButtonUp("option_0"))
			{
				interactions[0].Callback(interactions[0]);	
			}
			
			if(interactions.Count > 1)
			{
				if(Input.GetButtonUp("option_1"))
				{
					interactions[1].Callback(interactions[1]);	
				}	
			}
		}
		
		m_lastDirection = (Vector3.up * (Input.GetAxis("vertical_2"))) + (Vector3.right * (Input.GetAxis("horizontal_2")));
		float magnitude = m_lastDirection.magnitude;
		m_lastDirection.Normalize();
		
		Vector3 origin = transform.position;
		Vector3 playerDirection = transform.localRotation * Vector3.up;
		float diff = Mathf.Acos(Vector3.Dot(playerDirection, m_lastDirection)) * Mathf.Rad2Deg;
		

		
		m_objectsInView.Clear();
		
		if(magnitude > 0.0f && diff >= 0.0f && diff < InspectionAngleMax)
		{
			float targetAngle = -Mathf.Atan2(m_lastDirection.x, m_lastDirection.y)  * Mathf.Rad2Deg;
			
			float minAngle = targetAngle - InspectionAngleMax;
			float maxAngle = targetAngle + InspectionAngleMax;
			
			Vector3 min = Quaternion.Euler(0.0f, 0.0f, minAngle) * Vector3.up;
			Vector3 max = (Quaternion.Euler(0.0f, 0.0f, maxAngle) ) * Vector3.up;
			
			if(m_renderDebug)
			{
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.1f), transform.position + new Vector3(0.0f, 0.0f, -1.1f) + min , Color.green);
				Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.1f), transform.position + new Vector3(0.0f, 0.0f, -1.1f) + max , Color.green);
			}
			
			const float iterationCount = 10.0f;
			
			float sweepDelta = (InspectionAngleMax * 2.0f) / iterationCount;
			
			RaycastHit hitInfo;
			
			for(int sweepCount = 0; sweepCount < iterationCount; ++sweepCount)
			{
				float progress = minAngle + (sweepCount * sweepDelta);
				Vector3 currentDirection = (Quaternion.Euler(0.0f, 0.0f, progress) ) * Vector3.up;
				if(m_renderDebug)
				{
					Debug.DrawLine(transform.position + new Vector3(0.0f, 0.0f, -1.0f), transform.position + new Vector3(0.0f, 0.0f, -1.0f) + currentDirection, Color.red);
				}
				if(Physics.Raycast(transform.position, currentDirection, out hitInfo, 1.0f, collisionLayer))
				{
					InteractiveObject interactiveObject = hitInfo.collider.gameObject.GetComponent<InteractiveObject>();
					if(interactiveObject != null && !m_objectsInView.Contains(interactiveObject))
					{
						m_objectsInView.Add(interactiveObject);
					}
				}
			}
		}
	}
	
	List<InteractiveObject> m_objectsInRange 	= new List<InteractiveObject>();
	List<InteractiveObject> m_objectsInView		= new List<InteractiveObject>();
	
	private GUIStyle m_style;
	
	private int m_currentTab 		= 0;
	private Vector3 m_lastDirection = Vector3.zero;
}
