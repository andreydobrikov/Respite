using UnityEngine;
using System.Collections;

public class InspectionGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10.0f, 300.0f, 100.0f, 30.0f), "Inspection test");
	}
}
