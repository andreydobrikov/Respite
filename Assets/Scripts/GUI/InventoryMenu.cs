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

public class InventoryMenu : MonoBehaviour 
{
	public float delay = 0.2f;
	
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
	
	void SaveSerialise(List<SavePair> pairs)
	{
		
		pairs.Add(new SavePair("position_x", rigidbody.position.x.ToString()));
		pairs.Add(new SavePair("position_y", rigidbody.position.y.ToString()));
		pairs.Add(new SavePair("position_z", rigidbody.position.z.ToString()));
	}
	
	void SaveDeserialise(List<SavePair> pairs)
	{
		Vector3 position = Vector3.one;
		
		foreach(var pair in pairs)
		{
			if(pair.id == "position_x") { float.TryParse(pair.value, out position.x); }
			if(pair.id == "position_y") { float.TryParse(pair.value, out position.y); }
			if(pair.id == "position_z") { float.TryParse(pair.value, out position.z); }
		}
		rigidbody.position = position;
	}	
	
	public void OnGUI()
	{
		if(GameFlow.Instance.CurrentControlContext == GameFlow.ControlContext.Inventory)
		{
			GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, (Screen.height / 2) - 100, 300, 300));
		
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			GUILayout.Label("Inventory");
			GUILayout.Label("State: " + System.Enum.GetName(typeof(MenuState), m_state));
			
			int index = 0;
			foreach(var item in Inventory.Contents)
			{
				if(index == m_selectedIndex && Event.current.type == EventType.Repaint)
				{
					GUI.color = Color.red;	
					m_lastRect = GUILayoutUtility.GetLastRect();
				}
				
				GUILayout.Label(item.name);	
				
				GUI.color = Color.white;
				
				if(m_state == MenuState.Options)
				{
					GUI.depth = GUI.depth + 1;
					
					Rect lastRect = m_lastRect;
					lastRect.x += 10;
					lastRect.y += 30;
					lastRect.height = 100;
					
					GUILayout.BeginArea(lastRect);
					
					GUILayout.BeginVertical((GUIStyle)("Box"));
					
					List<Interaction> interactions = item.GetInteractions();
					
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
					
					foreach(var interaction in interactions)
					{
						GUILayout.Label(interaction.Name);	
					}
					
					GUILayout.EndVertical();
					
					GUILayout.EndArea();
				}
				
				index++;
			}
			
			GUILayout.EndVertical();
			
			GUILayout.EndArea();
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
	
	private enum MenuState
	{
		ItemSelect,
		Options
	}
}
