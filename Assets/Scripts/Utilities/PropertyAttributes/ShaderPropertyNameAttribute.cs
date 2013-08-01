using UnityEngine;
using System.Collections;

public class ShaderPropertyNameAttribute : PropertyAttribute 
{
	// This is a bit shit as it replicates the internal ShaderPropertyType enum and is therefore fucking unreliable
	public enum PropertyType
	{
		Color,
    	Vector,
    	Float,
    	Range,
    	TexEnv			
	}
	
	public ShaderPropertyNameAttribute(PropertyType propertyType)
	{
		ShaderPropertyType = propertyType;	
	}
	
	public PropertyType ShaderPropertyType = 0;
}
