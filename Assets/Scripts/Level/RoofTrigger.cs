using UnityEngine;
using System.Collections.Generic;

public class RoofTrigger : MonoBehaviour 
{
	public bool exitTrigger = true;
	public bool enterTrigger = true;

	public void SetRoofTrigger(Roof target)
	{
		m_targetRoofs.Add(target);
	}

	void OnTriggerEnter(Collider other)
	{
		// Not an entry trigger. Bail
		if(!enterTrigger)
		{
			return;
		}

		if(other.tag == "Player")
		{
			foreach(var roof in m_targetRoofs)
			{
				roof.TriggerEntered();
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		// Not an exit trigger. bail.
		if(!exitTrigger)
		{
			return;
		}

		if(other.tag == "Player")
		{
			foreach(var roof in m_targetRoofs)
			{
				roof.TriggerExited();
			}
		}
	}

	private List<Roof> m_targetRoofs = new List<Roof>();

}
