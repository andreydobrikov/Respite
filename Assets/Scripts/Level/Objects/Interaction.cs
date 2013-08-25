///////////////////////////////////////////////////////////
// 
// Interaction.cs
//
// What it does: Defines an interaction with an object in the world.
//
// Notes: Holds a context in which the interaction is available.
// 
// To-do:
//
///////////////////////////////////////////////////////////

using System.Collections;

public class Interaction 
{
	public delegate void InteractionHandler(Interaction source);
	
	public Interaction(string name, InteractionHandler handler, params ContextFlag[] flags)
	{
		m_name 		= name;
		m_handler 	= handler;
		
		foreach(var flag in flags)
		{
			m_flags = m_flags | (uint)flag;	
		}
	}
	
	public bool MatchesContext(uint flags)
	{
		return (m_flags & flags) != 0;
	}
	
	public string Name 					{ get { return m_name; } }
	public InteractionHandler Callback 	{ get { return m_handler; } }
	
	public bool Enabled
	{
		get { return m_enabled; }
		set { m_enabled = value; }
	}
	
	public uint Flags
	{
		get { return m_flags; }	
	}
	
	private string m_name 					= string.Empty;
	private InteractionHandler m_handler 	= null;
	private bool m_enabled 					= true;
	private uint m_flags					= 0;
}

public enum ContextFlag
{
	World 		= 0x1,
	Inventory 	= 0x2,
	
	All			= 0x3
}
