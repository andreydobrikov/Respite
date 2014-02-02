using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIBehaviourHeadFocus : AIBehaviour 
{
	public AIBehaviourHeadFocus()
	{
		m_name = "Head Track";
	}

	public override void RegisterBlackboardEntries()
	{
		m_headTrackBlackboardEntry = m_parentAI.Blackboard.AddEntry<Vector3>("headtrack_target", Vector3.forward);
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
		
		Vector3 target = Vector3.zero;
        target = m_headTrackBlackboardEntry.GetObject<Vector3>();

		Vector3 diff = target - m_parentAI.transform.position;
		diff.Normalize();

		float targetAngle = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;

		Quaternion targetRotation = Quaternion.Euler(0.0f, targetAngle, 0.0f);
		Quaternion currentRotation = m_headObject.transform.rotation;

		Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, m_lerpRate);

		m_headObject.transform.rotation = newRotation;
		 
	}

	public override void Shutdown()
	{

	}

#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        m_headObject = EditorGUILayout.ObjectField(m_headObject, typeof(GameObject), true) as GameObject;
    }
#endif

    [SerializeField]
    private GameObject m_headObject = null;

	private AIBlackBoardEntry m_headTrackBlackboardEntry = null;
	
	private float m_lerpRate = 0.2f;
}
