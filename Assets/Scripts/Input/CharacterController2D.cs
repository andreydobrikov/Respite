using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController2D : MonoBehaviour 
{
	public AnimationControl m_anim;
	public Sprite m_targetSprite = null;
	public float MoveSpeed = 200f;
	public float SprintIncrease = 100.0f;
	public float TurnSpeed = 10.0f;
	public float MoveAngle = 50.0f;	// The proximity of the player's direction to their target direction required before they can move. (degrees)
	
	public bool RenderDebugRays = false;
	
	Rigidbody m_controller = null;
	
	// Use this for initialization
	void Start () 
	{
		m_controller = GetComponent<Rigidbody>();
		m_gameFlow = GameFlow.Instance;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		// Reset the rigid-body's velocity to avoid inertia
		m_controller.velocity = Vector3.zero;
		
#if UNITY_EDITOR
		if(m_gameFlow == null)
		{
			m_gameFlow = GameFlow.Instance;		
		}
#endif
		
		if(m_gameFlow.CurrentControlContext == GameFlow.ControlContext.World)
		{
			Vector3 targetDirection = (Vector3.forward * (Input.GetAxis("Vertical"))) + (Vector3.right * (Input.GetAxis("Horizontal")));
			float sprintMultiplier = Input.GetAxis("sprint_analogue");
			
			float currentMoveSpeed = MoveSpeed + (Mathf.Sin(sprintMultiplier * Mathf.PI / 2.0f) * SprintIncrease);
			
			if(targetDirection.magnitude > 0.01f)
			{
				targetDirection.Normalize();
				
				Vector3 currentDirection = m_controller.rotation * Vector3.forward;
				
				float targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
				Quaternion newRotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
				
				targetDirection = newRotation * Vector3.forward;
				
				float diffAngle = Quaternion.Angle(newRotation, transform.localRotation);
					
				m_controller.MoveRotation(Quaternion.Slerp(transform.localRotation, newRotation, TurnSpeed / diffAngle));
				
				if(RenderDebugRays)
				{
					Debug.DrawLine(transform.position + new Vector3(0.0f, -1.0f, 0.0f), transform.position + ((Vector3)(currentDirection * 3.0f) + new Vector3(0.0f, -1.0f, 0.0f)), new Color(0.5f, 0.0f, 0.0f, 1.0f));
					Debug.DrawLine(transform.position + new Vector3(0.0f, -1.0f, 0.0f), transform.position + ((Vector3)(targetDirection * 3.0f) + new Vector3(0.0f, -1.0f, 0.0f)), Color.blue);		
				}
				
				if(diffAngle < MoveAngle)
				{
					Vector3 direction = (Vector3.forward * (Input.GetAxis("Vertical")));
					direction += (Vector3.right * (Input.GetAxis("Horizontal")));

					m_controller.AddForce( direction * currentMoveSpeed );
					if(m_anim != null)
					{
						m_anim.speed = direction.magnitude;
					}
				}
				else
				{
					if(m_targetSprite != null)
					{
						m_targetSprite.Play("turn_right");	
					}
				}
			}
			else
			{
				if(m_anim != null)
				{
					m_anim.speed = 0.0f;
				}
			}
		}
	}
	
	private GameFlow m_gameFlow = null;
}
