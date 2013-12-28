///////////////////////////////////////////////////////////
// 
// InventoryMenu.cs
//
// What it does: Provides an interface for the inventory system.
//
// Notes: This also handles serialisation of the inventory, as the inventory itself is not a MonoBehaviour
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class InventoryMenu : MonoBehaviour, ISerialisable
{
	public GUISkin skin;
	public float delay = 0.2f;

	void OnLevelWasLoaded()
	{

	}
	
	void Update()
	{
		if(GameFlow.Instance.CurrentControlContext == GameFlow.ControlContext.World)
		{
			if(Input.GetButtonDown("inventory"))
			{
				GameFlow.Instance.RequestInventory();	
				m_state = MenuState.ItemSelect;
			}
		}
		else if(GameFlow.Instance.CurrentControlContext == GameFlow.ControlContext.Inventory)
		{
			switch(m_state)
			{
				case MenuState.ItemSelect: { UpdateItemSelect(); break; }	
				case MenuState.Options: { UpdateOptions(); break; }	
			}
		}
	}
	
	private void UpdateItemSelect()
	{
		if(Input.GetButtonDown("inventory"))
		{
			GameFlow.Instance.EndInventory();	
		}
		
		if(Input.GetButtonDown("back"))
		{
			GameFlow.Instance.EndInventory();	
		}
		
		int itemCount = Inventory.Contents.Count;
		
		if(itemCount > 0)
		{
			if(Input.GetButtonDown("select"))
			{
				m_state = MenuState.Options;
				return;
			}
			
			if(m_transitionCooldown > 0.0f)
			{
				m_transitionCooldown -= Time.deltaTime;	
			}
			else
			{
				if(Input.GetAxis("Vertical") > 0.0f)
				{
					m_selectedIndex = (m_selectedIndex - 1) % itemCount;	
					if(m_selectedIndex < 0)
					{
						m_selectedIndex = itemCount + m_selectedIndex;	
					}
					
					m_transitionCooldown = delay;
				}
				else if(Input.GetAxis("Vertical") < 0.0f)
				{
					m_selectedIndex = (m_selectedIndex + 1) % itemCount;	
					m_transitionCooldown = delay;
				}	
			}
		}	
	}
	
	private void UpdateOptions()
	{
		if(Input.GetButtonDown("back"))
		{
			m_state = MenuState.ItemSelect;	
		}
		
		if(Input.GetButtonDown("inventory"))
		{
			GameFlow.Instance.EndInventory();	
		}
	}
	
	public void SaveSerialise(List<SavePair> pairs)
	{
		Inventory.SaveSerialise(pairs);
	}
	
	public void SaveDeserialise(List<SavePair> pairs)
	{
		Inventory.SaveDeserialise(pairs);
	}	
	
	public void OnGUI()
	{
		if(GameFlow.Instance.CurrentControlContext == GameFlow.ControlContext.Inventory)
		{
			Vector2 boxOrigin = new Vector2(Screen.width / 2 - 100, Screen.height / 2 - 100);
			
			GUILayout.BeginArea(new Rect(boxOrigin.x, boxOrigin.y, 300, 300));
		
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			GUILayout.Label("Inventory");
			GUILayout.Label("State: " + System.Enum.GetName(typeof(MenuState), m_state));
			
			int index = 0;
			foreach(var item in Inventory.Contents)
			{
				if(index == m_selectedIndex)
				{
					GUI.color = Color.red;	
				}
				
				GUILayout.Label(item.name);	
				
				if(index == m_selectedIndex)
				{
					if(Event.current.type == EventType.Repaint)
					{
						m_lastRect = GUILayoutUtility.GetLastRect();
						
						m_lastOrigin = new Vector2(m_lastRect.x, m_lastRect.y);
						m_lastOrigin = GUIUtility.GUIToScreenPoint(m_lastOrigin);
					}	
				}
				
				GUI.color = Color.white;
				index++;
				
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
			
			GUI.skin 			= skin;
			GUIStyle current 	= GUI.skin.GetStyle("Button");

			for (int i = 0; i < Inventory.Contents.Count; ++i)
			{
				if (i == m_selectedIndex)
				{
					GUILayout.BeginArea(new Rect(m_lastOrigin.x - 100, m_lastOrigin.y, 100, m_lastRect.height));
					GUILayout.Label("test", current);
					GUILayout.EndArea();
				}
			}
		}
		else
		{
			GUILayout.BeginArea(new Rect(Screen.width - 200, (Screen.height / 2) - 100, 180, 200));
		
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			GUILayout.Label("Inventory");
			
			foreach(var item in Inventory.Contents)
			{
				GUILayout.Label(item.name);	
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
		}
	
	}
	
	private float m_transitionCooldown = 0.0f;
	private int m_selectedIndex = 0;
	private MenuState m_state = MenuState.ItemSelect;
	private Rect m_lastRect;
	private Vector2 m_lastOrigin = Vector2.zero;
	
	private enum MenuState
	{
		ItemSelect,
		Options
	}
}
