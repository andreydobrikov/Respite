using System.Collections;

public class Interaction 
{
	public delegate void InteractionHandler(Interaction source);
	
	public Interaction(string name, InteractionHandler handler)
	{
		m_name 		= name;
		m_handler 	= handler;
	}
	
	public string Name 					{ get { return m_name; } }
	public InteractionHandler Callback 	{ get { return m_handler; } }
	
	public bool Enabled
	{
		get { return m_enabled; }
		set { m_enabled = value; }
	}
	
	private string m_name 					= string.Empty;
	private InteractionHandler m_handler 	= null;
	private bool m_enabled 					= true;
}
