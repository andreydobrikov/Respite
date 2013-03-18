using UnityEngine;
using System.Collections;

public abstract class WorldViewObject : MonoBehaviour
{
	protected WorldObject m_worldObject = null;
	
	public void SetWorldObject(WorldObject worldObject)
	{
		m_worldObject = worldObject;	
	}
	
	public virtual void OnStart() {}
}
