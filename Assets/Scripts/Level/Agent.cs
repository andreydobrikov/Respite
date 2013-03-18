using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour 
{

	private GameObject m_grabbedItem = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_grabbedItem != null)
		{
			m_grabbedItem.transform.position = transform.position;	
		}
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "TargetItem")
		{
			Debug.Log("Item taken!");
			m_grabbedItem = other.gameObject;
		}
		
		if(other.tag == "ExitPoint" && m_grabbedItem != null)
		{
			Debug.Log("Game Over");	
			//GameFlow.Instance.
		}
	}
}
