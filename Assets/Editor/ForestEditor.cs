using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Forest))] 
public class ForestEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		Forest forest = (Forest)target;
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		forest.m_activeInstanceCount	= EditorGUILayout.IntField("Active Instance Count", forest.m_activeInstanceCount);
		forest.m_treePrefab = EditorGUILayout.ObjectField("Tree Prefab", forest.m_treePrefab, typeof(GameObject), true) as GameObject;
		
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		
		int newSectionsX = EditorGUILayout.IntField("Sections X", forest.m_sectionsX);
		int newSectionsZ = EditorGUILayout.IntField("Sections Z", forest.m_sectionsZ);
		
		if((newSectionsX != forest.m_sectionsX || newSectionsZ != forest.m_sectionsZ) && newSectionsX > 0 && newSectionsZ > 0)
		{
			forest.SetSectionCounts(newSectionsX, newSectionsZ);
		}
		
		forest.m_treeRadius					= EditorGUILayout.FloatField("Tree Radius(fix)", forest.m_treeRadius);
		forest.m_colliderExpansionFactor 	= EditorGUILayout.FloatField("Collider Expansion Factor", forest.m_colliderExpansionFactor);
		forest.m_instanceCount 				= EditorGUILayout.IntField("World Tree Count", forest.m_instanceCount);
		forest.m_retryBailout				= EditorGUILayout.IntField("Generator Bailout Count", forest.m_retryBailout);
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		forest.m_showLayers = EditorGUILayout.Foldout(forest.m_showLayers, "Ignore Layers");
		
		if(forest.m_showLayers)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			for(int layerIndex = 0; layerIndex < forest.m_ignoreLayers.Count; ++layerIndex)
			{
				GUILayout.BeginHorizontal();
				
				forest.m_ignoreLayers[layerIndex] = EditorGUILayout.LayerField(forest.m_ignoreLayers[layerIndex]);
				if(GUILayout.Button("Delete"))
				{
					forest.m_ignoreLayers.RemoveAt(layerIndex);	
				}
				
				
				GUILayout.EndHorizontal();
			}
			
			if(GUILayout.Button("Add Layer"))
			{
				forest.m_ignoreLayers.Add(0);	
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndVertical();
		
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical((GUIStyle)("Box"));
		
		bool debugRender = EditorGUILayout.Toggle("Debug Rendering", forest.m_enabledDebugRendering);
		
		if(debugRender != forest.m_enabledDebugRendering)
		{
			forest.SetDebugRenderingEnabled(debugRender);	
		}
		
		if(forest.m_enabledDebugRendering)
		{
			GUILayout.BeginVertical((GUIStyle)("Box"));
			
			int newX = EditorGUILayout.IntSlider("X", forest.m_editorSectionX, 0, forest.m_sectionsX - 1);
			int newZ = EditorGUILayout.IntSlider("Z", forest.m_editorSectionZ, 0, forest.m_sectionsZ - 1);
			
			if(newX != forest.m_editorSectionX || newZ != forest.m_editorSectionZ)
			{
				forest.m_editorSectionX = newX;
				forest.m_editorSectionZ = newZ;
				EditorUtility.SetDirty(forest);	
			}
			
			GUILayout.EndVertical();
		}
			
		
		GUILayout.EndVertical();
		
		if( GUILayout.Button("Generate Forest"))
		{
			RegenerateForest();
		}
	}
		
	public void OnSceneGUI()
	{
		Forest forest = (Forest)target;
	
		float handleSize = HandleUtility.GetHandleSize(new Vector2(forest.m_startX, forest.m_startZ)) / 10.0f;
		
		Vector3 newMin = Handles.Slider2D(new Vector3(forest.m_startX, 0.0f, forest.m_startZ), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		Vector3 newMax = Handles.Slider2D(new Vector3(forest.m_endX, 0.0f, forest.m_endZ), new Vector3(0.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), handleSize, Handles.CubeCap, new Vector2(0.1f, 0.1f));
		
		forest.m_startX = Mathf.Min(newMin.x, newMax.x);
		forest.m_startZ = Mathf.Min(newMin.z, newMax.z);
		
		forest.m_endX = Mathf.Max(newMin.x, newMax.x);
		forest.m_endZ = Mathf.Max(newMin.z, newMax.z);
	}
	
	[MenuItem ("Respite/Forest/Regenerate")]
	static void RegenerateForest()
	{
		Forest forest = GameObject.FindObjectOfType(typeof(Forest)) as Forest;
			
		Selection.activeGameObject = forest.gameObject;
		
		int layerMask = ~0;
			int progressUpdateRate = forest.m_instanceCount / 200;
			
			foreach(var mask in forest.m_ignoreLayers)
			{
				layerMask &= ~(1 << mask);
				Debug.Log("Layer Mask: " + layerMask);
			}
			
			while(forest.transform.childCount > 0)
			{
				GameObject.DestroyImmediate(forest.transform.GetChild(0).gameObject);
			}
			
			forest.RebuildSections();
			bool cancel = false;
			int treesPlanted = 0;
			
			// TODO: This won't ever reach the target tree count as it gives up following a collision.
			for(int treeCount = 0; treeCount < forest.m_instanceCount && !cancel; ++treeCount)
			{
				bool succeeded = false;
				int counter = 0;
				
				while(!succeeded && counter < forest.m_retryBailout)
				{
					float x = Random.Range(forest.m_startX, forest.m_endX);
					float z = Random.Range(forest.m_startZ, forest.m_endZ);
				
					Vector3 position = new Vector3(x, 0.0f, z);
					
					bool overlap = Physics.CheckCapsule((Vector3)position + new Vector3(0.0f, -50.0f, 0.0f), (Vector3)position + new Vector3(0.0f, 50.0f, 0.0f), forest.m_treeRadius, layerMask);
					if(!overlap)
					{
						
							try
							{
								TreeInstance instance = new TreeInstance();
								instance.position = position;
								
								succeeded = !forest.AddInstance(instance);
								
								if(succeeded)
								{
									treesPlanted++;	
								}
							}
							catch(System.Exception e)
							{
								Debug.Log("Instance out of bound at " + position.x + ", " + position.z);
								Debug.Log(e);	
								Debug.Break();
							}
							
							counter++;
						
						
						if(treeCount % progressUpdateRate == 0)
						{
							cancel = EditorUtility.DisplayCancelableProgressBar("Generating Forest", "Creating trees (" + treeCount + ", " + forest.m_instanceCount + ")", (float)treeCount / (float)forest.m_instanceCount);
						}
					}
				}
			
				
					
			}
			Debug.Log("Planted " + treesPlanted + " trees");
			
			EditorUtility.ClearProgressBar();
	}
}
