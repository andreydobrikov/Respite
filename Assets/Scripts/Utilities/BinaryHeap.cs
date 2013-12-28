/// <summary>
/// Binary heap.
/// 
/// What the fuck. It's a fucking binary-heap.
/// 
/// </summary>

using UnityEngine;
using System.Collections.Generic;

public class BinaryHeap<T>
{
	public BinaryHeap(int maxEntries)
	{
		m_heapEntries = new BinaryHeapEntry<T>[maxEntries * 2];

		for(int i = 0; i < m_heapEntries.Length; i++)
		{
			m_heapEntries[i].m_metricValue = float.PositiveInfinity;
		}
	}
	
	public void Insert(T entry, float metricValue)
	{
		m_heapEntries[m_maxIndex].m_object 		= entry;
		m_heapEntries[m_maxIndex].m_metricValue = metricValue;
		m_maxIndex++;
		
		for(int pos = m_maxIndex - 1; pos > 0 && metricValue < m_heapEntries[(pos - 1) / 2].m_metricValue; pos = (pos - 1) / 2 )
		{
			BinaryHeapEntry<T> tmp = m_heapEntries[(pos - 1) / 2];
			m_heapEntries[(pos - 1) / 2] = m_heapEntries[pos];
			m_heapEntries[pos] = tmp;
		}
	}
	
	public T RemoveTop()
	{
		T top = m_heapEntries[0].m_object;

		if(m_maxIndex == 1)
		{
			m_heapEntries[0].m_metricValue = float.PositiveInfinity;
			m_maxIndex--;
			return top;
		}

		// Chuck the lowest head entry at the top
		m_heapEntries[0] = m_heapEntries[m_maxIndex - 1];

		// Mark the final entry with #inf just for safety
		m_heapEntries[m_maxIndex - 1].m_metricValue = float.PositiveInfinity;

		// Lower the count
		m_maxIndex--;
		
		float currentCost = m_heapEntries[0].m_metricValue;

		int currentIndex = 0;
		
		bool lowestFound = false;
		
		while(!lowestFound)
		{
			int index1 = (currentIndex + 1) * 2;
			int index2 = ((currentIndex + 1) * 2) - 1;

			float cost1 		= m_heapEntries[index1].m_metricValue;
			float cost2 		= m_heapEntries[index2].m_metricValue;

			// If the current value is less than both children, it's reached the correct position, so stop
			if(currentCost < cost1 && currentCost < cost2)
			{
				lowestFound = true;
				break;
			}

			// Select the smaller child and swap the two
			int switchIndex = cost1 < cost2 ? index1 : index2;
		
			BinaryHeapEntry<T> tmp = m_heapEntries[switchIndex];
			m_heapEntries[switchIndex] = m_heapEntries[currentIndex];
			m_heapEntries[currentIndex] = tmp;

			currentIndex = switchIndex;
		}

		return top;
	}
	
	public T GetTop()
	{
		return m_heapEntries[0].m_object;	
	}

	public float GetTopMetric()
	{
		return m_heapEntries[0].m_metricValue;
	}
		
	public List<string> OutputTree()
	{
		List<string> outputStrings = new List<string>();
		
		foreach(var entry in m_heapEntries)
		{
			outputStrings.Add(entry.m_metricValue.ToString());
		}
		
		return outputStrings;
	}
	
	public bool HasItems()
	{
		return m_maxIndex != 0;
	}
	
	public void Reset()
	{
		m_maxIndex = 0;
	}

	public string OutputGraph(bool toString)
	{
		int index = 0;
		int width = 1;
		Debug.Log("\nGraph Start");
		int numTabs = m_maxIndex /2;

		string currentLine = string.Empty;
		while(index < m_maxIndex)
		{

			for(int i = 0; i < width; i++)
			{
				for(int x = 0; x < numTabs; x++)
				{
					currentLine += "  ";
				}
				currentLine += "(" + index + ") " + m_heapEntries[index].m_metricValue.ToString("0.000") ;

				for(int x = 0; x < numTabs; x++)
				{
					currentLine += "  ";
				}


				index++;
			}
			numTabs /= 2;
			currentLine += "\n";

			width *= 2;

		}
		if(!toString)
			Debug.Log(currentLine);
		return currentLine;
	}
	
	private BinaryHeapEntry<T>[] m_heapEntries;
	private int m_maxIndex = 0;
	bool shitWentDown = false;
}

public struct BinaryHeapEntry<T>
{
	public T m_object;
	public float m_metricValue;
}
