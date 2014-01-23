using UnityEngine;
using System.Text;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerView))]
[AddComponentMenu("Custom/GUI/Interaction Menu")]
public class InteractionMenu : MonoBehaviour 
{
	public Font font;
	public GUISkin skin;
	
	public LayerMask collisionLayer 	= 0;
	public bool m_renderDebug			= false;
	public float InspectionFocusAngle	= 20.0f;
	
	void Start()
	{
		m_style 			= new GUIStyle();
		m_style.font 		= font;
		m_style.fontSize 	= 20;
		m_gameFlow 			= GameFlow.Instance;
		
		m_view 				= GameObject.FindObjectOfType(typeof(PlayerView)) as PlayerView;

		m_worldCollisionLayerID = LayerMask.NameToLayer("WorldCollision");

		m_layerMask = collisionLayer | (1 << m_worldCollisionLayerID);
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
#if UNITY_EDITOR
		if(m_gameFlow == null)
		{
			m_gameFlow = GameFlow.Instance;	
		}
#endif 
		
		if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.World)
		{
			return;	
		}
		
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
				List<Interaction> interactions = m_objectsInView[0].GetInteractions(ContextFlag.World);
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
						interactions[0].Callback(interactions[0], gameObject);	
					}
				}
					
				if(interactions.Count > 1)
				{
					GUI.enabled = interactions[1].Enabled;
					GUIStyle current = GUI.skin.GetStyle("Button");
					Vector2 size = current.CalcSize(new GUIContent(interactions[1].Name));
					
					if(GUI.Button(new Rect(centrePoint.x - 40.0f - size.x, centrePoint.y - 15.0f, size.x, 30.0f), interactions[1].Name))
					{
						interactions[1].Callback(interactions[1], gameObject);	
					}
				}
			}
		}
	}
	
	void Update()
	{
		
#if UNITY_EDITOR
		if(m_gameFlow == null)
		{
			m_gameFlow = GameFlow.Instance;	
		}
#endif
		
		if(m_gameFlow.CurrentControlContext != GameFlow.ControlContext.World)
		{
			return;	
		}

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
		
			List<Interaction> interactions = m_objectsInView[0].GetInteractions(ContextFlag.World);	
		
			if(Input.GetButtonUp("option_0") && interactions[0].Enabled)
			{
				interactions[0].Callback(interactions[0], gameObject);	
			}
			
			if(interactions.Count > 1)
			{
				if(Input.GetButtonUp("option_1") && interactions[1].Enabled)
				{
					interactions[1].Callback(interactions[1], gameObject);	
				}	
			}
		}
				
		for(int i = 0; i < m_objectsInView.Count; i++)
		{
			m_objectsInView[i].SetHighlightActive(false);
		}
		
		m_objectsInView.Clear();
		
		float minAngle = m_view.DirectionAngleDeg - InspectionFocusAngle;
		
		const float iterationCount = 10.0f;
		
		float sweepDelta = (InspectionFocusAngle * 2.0f) / iterationCount;
		
		for(int sweepCount = 0; sweepCount < iterationCount; ++sweepCount)
		{
			float progress = minAngle + (sweepCount * sweepDelta);
			Vector3 currentDirection = (Quaternion.Euler(0.0f, progress, 0.0f) ) * Vector3.forward;
			if(m_renderDebug)
			{
				Debug.DrawLine(transform.position + new Vector3(0.0f, -1.0f, 0.0f), transform.position + new Vector3(0.0f, -1.0f, 0.0f) + currentDirection, Color.red);
			}

			RaycastHit[] hits = Physics.RaycastAll(transform.position, currentDirection, 1.0f, m_layerMask);

			System.Array.Sort(hits, CompareHits);
			if(hits.Length > 0 && (hits[0].collider.gameObject.layer & m_worldCollisionLayerID) != 0)
			{
				continue;
			}
			else
			{
				foreach(var currentHit in hits)
				{
					InteractiveObject interactiveObject = currentHit.collider.gameObject.GetComponent<InteractiveObject>();
					if(interactiveObject != null && !m_objectsInView.Contains(interactiveObject) && interactiveObject.GetInteractions(ContextFlag.World).Count > 0)
					{
						m_objectsInView.Add(interactiveObject);
						interactiveObject.SetHighlightActive(true);
					}
				}
			}
		}
	}

	private static int CompareHits(RaycastHit hit0, RaycastHit hit1)
	{
		if(hit0.distance > hit1.distance)
		{
			return 1;
		}
		else if( hit0.distance < hit1.distance)
		{
			return -1;
		}

		return 0;
	}
	
	List<InteractiveObject> m_objectsInRange 	= new List<InteractiveObject>();
	List<InteractiveObject> m_objectsInView		= new List<InteractiveObject>();
	
	private GUIStyle m_style;
	private int m_currentTab 		= 0;
	
	private GameFlow m_gameFlow = null;
	private PlayerView m_view = null;

	private int m_worldCollisionLayerID = 0;
	private int m_layerMask = 0;
}
