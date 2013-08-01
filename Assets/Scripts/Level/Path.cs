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
	public bool m_drawPathOnly = false;
#endif
	
	
	public Spline m_spline;
	private bool m_initialised;
}
