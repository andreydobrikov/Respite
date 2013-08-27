///////////////////////////////////////////////////////////
// 
// InventoryObject.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SerialisableObject))]
[RequireComponent(typeof(InteractiveObject))]
public class InventoryObject : MonoBehaviour 
{
	public string ObjectName = "Inventory Item";
	
	public void Start()
	{
		m_interactiveObject = GetComponent<InteractiveObject>();
	}
	
	public List<Interaction> GetInteractions()
	{
		return m_interactiveObject.GetInteractions(ContextFlag.Inventory);	
	}
	
	private InteractiveObject m_interactiveObject = null;
}
