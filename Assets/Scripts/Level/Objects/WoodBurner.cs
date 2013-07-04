using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Custom/Interactive Objects/WoodBurner")]
public class WoodBurner : InteractiveObject
{
	public override List<Interaction> GetInteractions()
	{
		List<Interaction> interactions = new List<Interaction>();
		interactions.Add(new Interaction("Stoke", new Interaction.InteractionHandler(HandleStoke)));
		
		return interactions;
	}
			
	private void HandleStoke(Interaction interaction)
	{
		
	}
}
