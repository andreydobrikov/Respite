/// <summary>
/// Custom property-drawer for tag-values.
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer (typeof (TagAttribute))]
public class TagDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) 
	{
		prop.stringValue = EditorGUI.TagField(new Rect (pos.x, pos.y, pos.width, pos.height), "Tag", prop.stringValue);
    }
}
