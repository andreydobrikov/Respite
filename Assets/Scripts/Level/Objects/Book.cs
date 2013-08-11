using UnityEngine;
using System.Collections;

public class Book : InteractiveObject
{
	public Book()
	{
		m_takeInteraction = new Interaction("Take Book", TakeBookHandler);
		
		m_interactions.Add(m_takeInteraction);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void TakeBookHandler(Interaction source)
	{
		Debug.Log("Book Taken");
		GameFlow.Instance.RequestInspection();
	}
	
	Interaction m_takeInteraction;
}
