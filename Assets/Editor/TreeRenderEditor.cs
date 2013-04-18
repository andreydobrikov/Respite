using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TreeRender))]
public class TreeRenderEditor : Editor 
{
	public override void OnInspectorGUI() 
	{
		TreeRender tree = (TreeRender)target;
		
		Color newColour = EditorGUILayout.ColorField("Base Colour", tree.baseColour);
		float newDuration = EditorGUILayout.FloatField("Anim Duration", tree.animDuration);
		
		if(newColour != tree.baseColour || newDuration != tree.animDuration)
		{
			tree.baseColour = newColour;
			tree.animDuration = newDuration;
			
			tree.UpdateBaseMaterial();
		}
	}
}
