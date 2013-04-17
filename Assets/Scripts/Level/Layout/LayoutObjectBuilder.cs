using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutObjectBuilder
{
	public void BuildObjects(Level level)
	{
		GameObject generatedItemsObject = GetGeneratedItemsObject(level);
		
		while(generatedItemsObject.transform.childCount > 0)
		{
			GameObject.DestroyImmediate(generatedItemsObject.transform.GetChild(0).gameObject);
		}
		
		// Mark all connections as dirty
		foreach(var node in level.Nodes)
		{
			node.SetOwner(level); // Remove
			foreach(var connection in node.ConnectedNodes)
			{
				connection.Built = false;
			}
		}
		
		foreach(var node in level.Nodes)
		{
			
			List<GameObject> newObjects = node.BuildObject();	
			
			foreach(var newObject in newObjects)
			{
				newObject.transform.parent = generatedItemsObject.transform;	
			}
		}
	}
	
	/// <summary>
	/// Gets the Level's "generated_items" child.
	/// </summary>
	/// <returns>
	/// The existing "generated_items" child, whether pre-existing or created here.
	/// </returns>
	/// <param name='level'>
	/// The level script.
	/// </param>
	private GameObject GetGeneratedItemsObject(Level level)
	{
		GameObject levelObject = level.gameObject;
		
		Transform meshesTransform =  levelObject.transform.FindChild("generated_items");
		GameObject meshesObject = null;
		
		if(meshesTransform == null)
		{
			meshesObject = new GameObject("generated_items");
			meshesObject.transform.parent = levelObject.transform;
		}
		else
		{
			meshesObject = meshesTransform.gameObject;	
		}
		
		return meshesObject;
	}
	
}
