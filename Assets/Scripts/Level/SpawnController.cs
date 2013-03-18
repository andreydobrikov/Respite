using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnController
{
	public static SpawnController Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = new SpawnController();
			}
			return m_instance;
		}
	}
	
	public void RegisterSpawner(Spawner spawner)
	{
		m_spawners.Add(spawner);
	}
	
	public void SpawnRandom(string tag)
	{
		List<Spawner> eligibleSpawners = new List<Spawner>();
		
		foreach(Spawner spawner in m_spawners)
		{
			if(spawner.spawnObject.tag == tag)
			{
				eligibleSpawners.Add(spawner);	
			}
		}
		
		if(eligibleSpawners.Count > 0)
		{
			int spawnerIndex = (int)(eligibleSpawners.Count * Random.value);
		
			eligibleSpawners[spawnerIndex].Spawn();	
		}
	}
	
	private List<Spawner> m_spawners = new List<Spawner>();
	
	private static SpawnController m_instance = null;
}
