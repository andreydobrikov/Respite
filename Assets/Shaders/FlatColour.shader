Shader "Custom/FlatColour" 
{
	Properties 
	{
	    _Color ("Main Color", Color) = (1, 1, 1, 1)
	
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		cull off
	
	    Pass 
	    {
		//	Blend DstAlpha DstAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;
			
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    fixed4 color : COLOR;
			};
			
			v2f vert (appdata_full v) 
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.color = v.color;
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    return _Color * i.color.a;
			}
			ENDCG
	
	    }
	}
	Fallback "VertexLit"
}
