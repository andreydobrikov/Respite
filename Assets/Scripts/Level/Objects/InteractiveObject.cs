using UnityEngine;
using System.Collections.Generic;

public abstract class InteractiveObject : MonoBehaviour 
{
	public abstract List<Interaction> GetInteractions();
}
