///////////////////////////////////////////////////////////
// 
// AIEditor.cs
//
// What it does: The inspector portion of the AI-editor.
//
// Notes: 
// 
// To-do:
//
///////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AI))] 
public class AIEditor : Editor
{
	public override void OnInspectorGUI()
	{
		AI ai = (AI)target;

        var types = GetAvailableTypes(ai);

		string[] typeStrings = new string[types.Count];

		int index = 0;
		foreach(var currentType in types)
		{
			typeStrings[index] = currentType.Name;
			index++;
		}

		GUILayout.BeginHorizontal();

		if(types.Count == 0)
		{
			GUI.enabled = false;
		}

		ai.m_selectedBehaviourIndex = Mathf.Min(EditorGUILayout.Popup(ai.m_selectedBehaviourIndex, typeStrings), Mathf.Max(0, types.Count - 1));

		if(GUILayout.Button("Add", GUILayout.Width(50)))
		{
			ai.Behaviours.Add(ScriptableObject.CreateInstance(types[ai.m_selectedBehaviourIndex]) as AIBehaviour);
		}

		GUI.enabled = true;

		GUILayout.EndHorizontal();
		List<AIBehaviour> toDelete = new List<AIBehaviour>();

        EditorGUILayout.BeginVertical((GUIStyle)("Box"));

		foreach(var behaviour in ai.Behaviours)
		{
			behaviour.m_showFoldout = EditorGUILayout.Foldout(behaviour.m_showFoldout, behaviour.Name);

            if(behaviour.m_showFoldout)
            {
                behaviour.OnInspectorGUI();
            }
		}

        EditorGUILayout.EndVertical();

		foreach(var deletedBehaviour in toDelete)
		{
			ai.Behaviours.Remove(deletedBehaviour);
		}
	}

    // Returns a list of AIBehaviours not yet used by a given AI.
    private List<System.Type> GetAvailableTypes(AI ai)
    {
        System.Type targetType = typeof(AIBehaviour);

        // Get all types that implement AIBehaviour
        List<System.Type> types = new List<System.Type>(targetType.Assembly.GetTypes().Where(x => x.IsSubclassOf(targetType)));

        // Create a list of the types already used
        List<System.Type> aiTypes = new List<System.Type>();
        foreach(var aiType in ai.Behaviours)
        {
            aiTypes.Add(aiType.GetType());
        }

        // Repopulate the types list with only those types that are unused
        types = new List<System.Type>(types.Where(x => !aiTypes.Contains(x)));

        return types;
    }
}

