using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Interactive Objects/WoodBurner")]
public class WoodBurner : InteractiveObject
{
	void Start()
	{
		m_interactions.Add(new Interaction("Stoke", new Interaction.InteractionHandler(HandleStoke)));
	}
	
	private void HandleStoke(Interaction interaction, GameObject trigger)
	{
		
	}
}
