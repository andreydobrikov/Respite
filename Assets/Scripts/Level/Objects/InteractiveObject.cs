#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public abstract class InteractiveObject : MonoBehaviour 
{
	public List<Interaction> GetInteractions()
	{
		return m_interactions;	
	}
	
#if UNITY_EDITOR
	public void GenerateHighlight(bool deleteExisting)
	{
		GameObject highlightObject = GameObjectHelper.FindChild(this.gameObject, s_highlightObjectName, true);
		
		if(highlightObject != null && deleteExisting)
		{
			DestroyImmediate(highlightObject);
		}
		
		if(highlightObject == null)
		{
			highlightObject = new GameObject(s_highlightObjectName);
			highlightObject.layer = LayerMask.NameToLayer("Overlay");
			highlightObject.transform.parent = transform;
			highlightObject.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
			highlightObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			
			MeshRenderer renderer 		= highlightObject.AddComponent<MeshRenderer>();
			MeshFilter filter 			= highlightObject.AddComponent<MeshFilter>();
			GeometryFactory geoFactory  = highlightObject.AddComponent<GeometryFactory>();
			Highlight highlight			= highlightObject.AddComponent<Highlight>();
			
			renderer.material = AssetHelper.Instance.GetAsset<Material>("materials/highlight.mat") as Material;
			
			GameObject parentRenderer = GameObjectHelper.SearchForComponent(highlightObject, typeof(MeshRenderer));
			
			if(parentRenderer != null)
			{
				renderer.material.mainTexture = parentRenderer.renderer.sharedMaterial.mainTexture;
				Debug.Log("Parent renderer found at " + parentRenderer.name + " | " + renderer.sharedMaterial.mainTexture.name);	
			}
			else
			{
				Debug.Log("No parent renderer found for " + highlightObject.transform.parent.name);	
			}
			
			m_highlight = highlight;
			m_highlight.Deactivate();
		}
	}
	
	public void SetHighlightActive(bool active)
	{
		if(m_highlight != null)
		{
			if(active)
			{
				m_highlight.Activate();
			}
			else
			{
				m_highlight.Deactivate();	
			}
		}
	}
	
	[MenuItem ("Respite/Interactive Objects/Rebuild Highlights")]
	static void RebuildHighlights () 
	{
		bool deleteCurrent = EditorUtility.DisplayDialog("Delete Existing Highlights?", "Would you like to rebuild all highlights? Huh?", "yes", "no");
		
		InteractiveObject[] interactiveObjects = FindObjectsOfType (typeof(InteractiveObject)) as InteractiveObject[];
		
		foreach(var currentObject in interactiveObjects)
		{
			currentObject.GenerateHighlight(deleteCurrent);
		}
		
		Debug.Log("Highlights Generated: " + interactiveObjects.Length);
	}
	
#endif
	
	protected List<Interaction> m_interactions = new List<Interaction>();
	
	private static string s_highlightObjectName = "highlight";
	
	[SerializeField]
	private Highlight m_highlight = null;
}
