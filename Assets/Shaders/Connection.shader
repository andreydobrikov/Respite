Shader "Custom/Connection" 
{
	Properties 
	{
	    _StartColor ("Start Color", Color) = (1,1,1,1)
		_EndColor ("End Color", Color) = (1, 1, 1, 1)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
	
	    Pass 
	    {
		//	Blend DstAlpha DstAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _StartColor;
			float4 _EndColor;
			
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = v.texcoord;
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    
			    return lerp(_StartColor, _EndColor, i.uv.x);
			}
			ENDCG
	
	    }
	}
	Fallback "VertexLit"
}
