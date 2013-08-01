/// <summary>
/// Custom property-drawer for setting a string to a shader's property-name.
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer (typeof (ShaderPropertyNameAttribute))]
public class ShaderPropertyNameDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) 
	{
		// Check there's a shader to extract properties from
		GameObject gameObject = (prop.serializedObject.targetObject as MonoBehaviour).gameObject;
		if(gameObject != null)
		{
			MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
			if(renderer == null)
			{
				EditorGUI.LabelField(new Rect (pos.x, pos.y, pos.width, pos.height), "No MeshRenderer found");
			}
			else if(renderer.sharedMaterial == null)
			{
				EditorGUI.LabelField(new Rect (pos.x, pos.y, pos.width, pos.height), "No Material found");
			}
			else if(renderer.sharedMaterial.shader == null)
			{
				EditorGUI.LabelField(new Rect (pos.x, pos.y, pos.width, pos.height), "No suitable shader found");
			}
			else
			{
				ShaderPropertyNameAttribute parameter 	= (ShaderPropertyNameAttribute)attribute;
				Shader shader 							= renderer.sharedMaterial.shader;
				int propertyCount 						= ShaderUtil.GetPropertyCount(shader);
				int selectedTexture 					= 0;
				List<string> textureNames 				= new List<string>(); // TODO: No need to instantiate this each iteration
				
				EditorGUI.LabelField(new Rect (pos.x, pos.y, pos.width / 2.0f, 20.0f), label.text + " (" + parameter.ShaderPropertyType.ToString() + ")");	
				
				// Loop through the parameters adding those that match type to the list
				for(int propertyID = 0; propertyID < propertyCount; ++propertyID)
				{
					// TODO: It would be lovely to mirror the internal enum somehow...
					if(ShaderUtil.GetPropertyType(shader, propertyID) == (ShaderUtil.ShaderPropertyType)(parameter.ShaderPropertyType))
					{
						string name = ShaderUtil.GetPropertyName(shader, propertyID);
						
						// If the shader-property's name matches the current value of the SerializedProperty, update the selected index.
						if(name == prop.stringValue)
						{
							selectedTexture = textureNames.Count;
						}
						
						textureNames.Add(name);
					}
				}
				
				if(textureNames.Count > 0)
				{
					int newVal = EditorGUI.Popup(new Rect (pos.x + pos.width / 2.0f, pos.y, pos.width / 2.0f, pos.height), selectedTexture, textureNames.ToArray());
					prop.stringValue = textureNames[newVal];
				}
				else
				{
					EditorGUI.LabelField(new Rect (pos.x + pos.width / 2.0f, pos.y, pos.width / 2.0f, pos.height), "No Properties Found");	
				}
			}
		}
    }
}
