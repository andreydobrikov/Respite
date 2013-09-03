///////////////////////////////////////////////////////////
// 
// ISerialisable.cs
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

public interface ISerialisable
{
	void SaveSerialise(List<SavePair> pairs);
	void SaveDeserialise(List<SavePair> pairs);
}
