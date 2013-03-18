using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour 
{
	public GameObject spawnObject;
	
	void Start()
	{
		SpawnController.Instance.RegisterSpawner(this);	
	}

	public void Spawn()
	{
		GameObject newObject = Instantiate(spawnObject) as GameObject;
		newObject.transform.position = transform.position;
	}
}
