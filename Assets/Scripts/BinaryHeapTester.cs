using UnityEngine;
using System.Collections.Generic;

public class BinaryHeapTester : MonoBehaviour {


	public bool DoWork = false;
	const int numThings = 200;
	// Use this for initialization
	void Start () {
		m_heap = new BinaryHeap<Vector3>(400);

		for(int i =0; i < numThings; i++)
		{
			m_vals.Add(Random.Range(-1.0f, 1.0f));
		}
		m_vals[4] = m_vals[3] = m_vals[2];
	}
	
	// Update is called once per frame
	void Update () {
		if(DoWork)
		{
			DoWork = false;
			Queue<string> heaps = new Queue<string>();

			m_heap.Reset();
			for(int i = 0; i < numThings; i++)
			{
				m_heap.Insert(Vector3.zero, m_vals[i]);
			}

			m_heap.OutputGraph(false);
			float lastMetric = float.NegativeInfinity;

			string lastHeap = string.Empty;
			while(m_heap.HasItems())
			{
				float top = m_heap.GetTopMetric();
				if(top < lastMetric)
				{
					m_heap.OutputGraph(false);
					Debug.Log("\nLAST GRAPHs\n");
					while(heaps.Count > 0)
					{
						Debug.Log("\n" + heaps.Dequeue());
					}
				}

				m_heap.RemoveTop();
				Debug.Log(top);
				lastMetric = top;
				heaps.Enqueue(m_heap.OutputGraph(true));

				//if(heaps.Count > 10)
				{
				//	heaps.Dequeue();
				}
			}


		}
	}

	private List<float> m_vals = new List<float>();

	private BinaryHeap<Vector3> m_heap;
}
