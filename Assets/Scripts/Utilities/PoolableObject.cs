///////////////////////////////////////////////////////////
// 
// PoolableObject.cs
//
// What it does: 
//
// Notes:
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class PoolableObject : MonoBehaviour 
{
	public void SetPool(ObjectPool pool)
	{
		m_pool = pool;
	}
	
	protected ObjectPool m_pool;
}
