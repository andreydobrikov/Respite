using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderMeshHelper : EditorWindow
{

	[MenuItem("Respite/Collider Meshes/BuildColliderMeshes")]
	static void BuildColliderMeshes()
	{
		foreach(var current in Selection.objects)
		{
			GameObject currentObject = current as GameObject;
			
			if(currentObject.transform.childCount == 0)
			{
				GameObject newObject = new GameObject("Mesh");
				newObject.tag = "EditorOnly";
				newObject.layer = currentObject.layer;
				
				newObject.AddComponent<GeometryFactory>();
				var meshRenderer = newObject.AddComponent<MeshRenderer>();
				newObject.AddComponent<ColliderMesh>();
				
				meshRenderer.sharedMaterial = AssetHelper.Instance.FindAsset<Material>("book.mat") as Material;
				
				newObject.transform.parent = currentObject.transform;
				
				newObject.transform.localPosition = Vector3.zero;
				newObject.transform.localRotation = Quaternion.identity;
				newObject.transform.localScale = Vector3.one;
			}
			
		}
	}

	[MenuItem("Respite/Collider Meshes/Hide All")]
	static void HideColliderMeshes()
	{
		ColliderMesh[] colliderMeshes = Resources.FindObjectsOfTypeAll(typeof(ColliderMesh)) as ColliderMesh[];

		foreach(var mesh in colliderMeshes)
		{
			mesh.gameObject.SetActive(false);
		}
	}

	[MenuItem("Respite/Collider Meshes/Show All")]
	static void ShowColliderMeshes()
	{
		ColliderMesh[] colliderMeshes = Resources.FindObjectsOfTypeAll(typeof(ColliderMesh)) as ColliderMesh[];

		foreach(var mesh in colliderMeshes)
		{ 
			mesh.gameObject.SetActive(true);
		}
	}

	[MenuItem("Respite/Collider Meshes/Delete All")]
	static void DeleteColliderMeshes()
	{
		ColliderMesh[] colliderMeshes = Resources.FindObjectsOfTypeAll(typeof(ColliderMesh)) as ColliderMesh[];
		
		foreach(var mesh in colliderMeshes)
		{
			DestroyImmediate(mesh.gameObject);
		}
	}

	void OnGUI()
	{

	}

}
