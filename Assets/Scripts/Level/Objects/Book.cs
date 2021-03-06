using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InventoryObject))]
public class Book : InteractiveObject
{
	public Book()
	{
		m_takeInteraction 		= new Interaction("Take Book", TakeBookHandler, ContextFlag.World);
		m_inspectInteraction 	= new Interaction("Inspect", InspectBookHandler, ContextFlag.All);
		
		m_interactions.Add(m_takeInteraction);
		m_interactions.Add(m_inspectInteraction);
	}
	
	// Use this for initialization
	void Start () 
	{
		m_inventoryObject = GetComponent<InventoryObject>();
	}
	
	void TakeBookHandler(Interaction source, GameObject trigger)
	{
		Debug.Log("Book Taken");
		Inventory.AddToInventory(m_inventoryObject);
	}
		
	void InspectBookHandler(Interaction source, GameObject trigger)
	{
		GameFlow.Instance.RequestInspection();
	}
	
	Interaction m_takeInteraction 		= null;
	Interaction m_inspectInteraction 	= null;
	
	InventoryObject m_inventoryObject 	= null;
}
