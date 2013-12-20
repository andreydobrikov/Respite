using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

public class TagSearch : EditorWindow {

	[MenuItem ("Respite/Misc/TagSearch")]
	static void DoSearch()
	{
		EditorWindow.GetWindow(typeof(TagSearch));
	}

    void OnGUI()
	{
		if(string.IsNullOrEmpty(s_tagName))
		{
			s_tagName = UnityEditorInternal.InternalEditorUtility.tags[0];
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Tag", GUILayout.Width(50));
		s_tagName = EditorGUILayout.TagField(s_tagName);

		if(GUILayout.Button("Search", GUILayout.Width(100)))
		{

			s_objects = GameObject.FindGameObjectsWithTag(s_tagName);


		}

		GUILayout.EndHorizontal();

		if(s_objects != null)
		{
			s_scroll = GUILayout.BeginScrollView(s_scroll);
			GUILayout.BeginVertical();

			foreach(var thing in s_objects)
			{
				if(GUILayout.Button(thing.name))
				{
					Selection.activeObject = thing;
				}
			}

			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}

	private static string s_tagName = string.Empty;
	private static Vector2 s_scroll = Vector2.zero;
	private GameObject[] s_objects = null;
}

//public class TextInputField
