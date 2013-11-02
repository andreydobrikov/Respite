///////////////////////////////////////////////////////////
// 
// Interaction.cs
//
// What it does: Defines an interaction with an object in the world.
//
// Notes: Holds a context in which the interaction is available.
//
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class Interaction 
{
	public delegate void InteractionHandler(Interaction source, GameObject trigger);
	
	public Interaction(string name, InteractionHandler handler, params ContextFlag[] flags)
	{
		m_name 		= name;
		m_handler 	= handler;
		
		foreach(var flag in flags)
		{
			m_flags = m_flags | (uint)flag;	
		}
		
		if(flags.Length == 0)
		{
			m_flags = (uint)ContextFlag.All;	
		}
	}
	
	public bool MatchesContext(uint flags)
	{
		return (m_flags & flags) != 0;
	}
	
	#region Properties
	
	public string Name 					{ get { return m_name; } }
	public InteractionHandler Callback 	{ get { return m_handler; } }
	
	public bool Enabled
	{
		get { return m_enabled; }
		set { m_enabled = value; }
	}
	
	public bool Hidden
	{
		get { return m_hidden; }
		set { m_hidden = value; }
	}
	
	public uint Flags
	{
		get { return m_flags; }	
	}
	
	#endregion
	
	private string m_name 					= string.Empty;
	private InteractionHandler m_handler 	= null;
	private bool m_enabled 					= true;
	private bool m_hidden 					= true;
	private uint m_flags					= 0;
}

public enum ContextFlag
{
	None		= 0x0, 
			
	World 		= 0x1,
	Inventory 	= 0x2,
	
	All			= 0x3
}
