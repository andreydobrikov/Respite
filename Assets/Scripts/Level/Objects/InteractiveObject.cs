#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;

public abstract class InteractiveObject : MonoBehaviour, ISerialisable
{
	public List<Interaction> GetInteractions()
	{
		return m_interactions;	
	}
	
	public List<Interaction> GetInteractions(params ContextFlag[] flags)
	{
		uint allFlags = 0;
		foreach(var flag in flags)
		{
			allFlags = allFlags | (uint)flag;	
		}
		
		
		return m_interactions.FindAll(x => x.MatchesContext(allFlags));
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
			
			highlightObject.AddComponent<MeshFilter>();
			highlightObject.AddComponent<GeometryFactory>();
			
			MeshRenderer renderer 		= highlightObject.AddComponent<MeshRenderer>();
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
	
	public virtual void SaveSerialise(List<SavePair> pairs)
	{
		Debug.Log("Save Serialise");
		
		Vector3 position = transform.position;
		
		pairs.Add(new SavePair("position_x", position.x.ToString()));
		pairs.Add(new SavePair("position_y", position.y.ToString()));
		pairs.Add(new SavePair("position_z", position.z.ToString()));
	}
	
	public virtual void SaveDeserialise(List<SavePair> pairs)
	{
		Vector3 position = Vector3.zero;
		
		foreach(var pair in pairs)
		{
			if(pair.id == "position_x") float.TryParse(pair.value, out position.x);		
			if(pair.id == "position_y") float.TryParse(pair.value, out position.y);
			if(pair.id == "position_z") float.TryParse(pair.value, out position.z);
		}
		
		transform.position = position;
	}
	
	protected List<Interaction> m_interactions = new List<Interaction>();
	
	private static string s_highlightObjectName = "highlight";
	
	[SerializeField]
	private Highlight m_highlight = null;
}
