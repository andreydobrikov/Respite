using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Path : MonoBehaviour 
{
	public Path()
	{
		m_initialised = false;
		m_spline = new Spline();
	}
	
	void OnEnable()
	{
		if(!m_initialised)
		{
			m_spline.Start();
			// The splines will default to being at the origin, so shift them to the position of the path.
			m_spline.Offset(transform.position);
			
			m_initialised = true;
		}
	}
	
#if UNITY_EDITOR
	
	public float m_uv0Multiplier			= 1.0f;
	public float m_uv1Multiplier			= 1.0f;
	public float m_meshWidth				= 0.2f;
	public float m_meshDepth				= -1.0f;
	public Material m_meshMaterial			= null;
	public int m_meshSegmentCount			= 30;
	public int m_meshLayer					= 0;
	
	public int m_colliderSectionCount 		= 1;
	public float m_colliderWidthMultiplier 	= 1.0f;
	
	public bool m_showCollidersFoldout		= false;
	public bool m_showMeshesFoldout			= false;
	public bool m_showGeneratorFoldout 		= false;
	public bool m_colliders					= true;
	public bool m_meshes					= true;
	public bool m_drawPathOnly 				= false;
#endif
	
	
	public Spline m_spline;
	
	[SerializeField]
	private bool m_initialised;
}
