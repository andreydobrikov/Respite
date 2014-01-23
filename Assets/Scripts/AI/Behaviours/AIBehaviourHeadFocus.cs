using UnityEngine;
using System.Collections;

public class AIBehaviourHeadFocus : AIBehaviour 
{
	public AIBehaviourHeadFocus()
	{
		m_name = "Head Track";
	}

	public override void RegisterBlackboardEntries()
	{
		m_headTrackBlackboardID = m_parentAI.Blackboard.AddEntry<Vector3>("headtrack_target", Vector3.forward);
	}

	public override void Start()
	{
		var headTransform = m_parentAI.transform.FindChild("head");

		if (headTransform != null)
		{
			m_headObject = headTransform.gameObject;
		}
	}

	public override void Update()
	{
		/*
		Vector3 target = Vector3.zero;
		m_parentAI.Blackboard.GetEntry<Vector3>(m_headTrackBlackboardID, ref target);

		Vector3 diff = target - m_parentAI.transform.position;
		diff.Normalize();

		float targetAngle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;

		Quaternion targetRotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
		Quaternion currentRotation = m_headObject.transform.rotation;

		Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, m_lerpRate);

		m_headObject.transform.rotation = newRotation;
		 */
	}

	public override void Shutdown()
	{

	}

	private int m_headTrackBlackboardID = -1;
	private GameObject m_headObject = null;
	private float m_lerpRate = 0.2f;
}
