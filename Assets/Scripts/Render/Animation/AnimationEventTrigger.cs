using UnityEngine;
using System.Collections;

public class AnimationEventTrigger : MonoBehaviour {

	public AnimationEventResponder Responder = null;

	public void IssueEvent()
	{
		if(Responder != null)
		{
			Responder.HandleEvent();
		}
	}
}
