using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	public int m_x;
	public int m_y;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnDestroy()
	{
	}
	
	public int X { get { return m_x; } set { m_x = value; } }
	public int Y { get { return m_y; } set { m_y = value; } }
}
