
using UnityEngine;

public enum WorldView
{
	Agent,
	Admin
}

public abstract class WorldObject : MonoBehaviour
{
	void Start()
	{
		WorldView view = GameFlow.Instance.View;
		CreateViewObject(view);
		
		if(m_viewObject != null)
		{
			m_viewObject.OnStart();
		}
		
		OnStart();
	}
	
	public void OnDestroy()
	{
		Destroy(m_gameObject);	
	}
	
	public void CreateViewObject(WorldView view)
	{
		switch(view)
		{
			case WorldView.Agent: 	if( m_agentViewPrefab != null) m_gameObject = UnityEngine.GameObject.Instantiate(m_agentViewPrefab) as GameObject; break;
			case WorldView.Admin: 	if( m_adminViewPrefab != null) m_gameObject = UnityEngine.GameObject.Instantiate(m_adminViewPrefab) as GameObject; break;
		}
		
		if(m_gameObject == null)
		{
			Debug.LogError("Failed to create " + System.Enum.GetName(typeof(WorldView), GameFlow.Instance.View) + " for type: " + name);
			return;
		}
		
		m_viewObject = m_gameObject.GetComponent<WorldViewObject>();
		
		if(m_viewObject == null)
		{
			Debug.LogError("Cannot add view of type: " + m_gameObject.name + " as the type does not contain a WorldViewObject script");
			m_gameObject = null;
			
			return;
		}
		
		m_viewObject.SetWorldObject(this);
		//m_gameObject.name = ;
		
		m_gameObject.transform.parent 	= transform;
		m_gameObject.transform.position = transform.position;
		m_gameObject.transform.rotation = transform.rotation;
	}
	
	protected virtual void OnStart()
	{
		
	}
	
	public GameObject m_agentViewPrefab;
	public GameObject m_adminViewPrefab;
	
	protected GameObject 		m_gameObject = null;
	protected WorldViewObject 	m_viewObject = null;
	
}
