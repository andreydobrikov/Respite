using UnityEngine;
using System.Collections.Generic;

public abstract class InteractiveObject : MonoBehaviour 
{
	public List<Interaction> GetInteractions()
	{
		return m_interactions;	
	}
	
	protected List<Interaction> m_interactions = new List<Interaction>();
}
