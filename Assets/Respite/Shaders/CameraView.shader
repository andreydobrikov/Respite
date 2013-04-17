Shader "Custom/CameraView" 
{
	Properties 
	{
	    _Color ("Main Color", Color) = (1,1,0,1.0)
	
	}
	
	SubShader 
	{
		Tags {"Queue" = "Transparent" }
	    Pass 
	    { 
	    	
	    	//ZWrite Off
	    ColorMask RGB
	    //BlendOp Min
			Blend SrcAlpha  OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _Color;
			
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2 uv_MainTex : TEXCOORD0;
			};
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 color = _Color;
			    return _Color;
			}
		
			ENDCG
	
	    }
	}
	Fallback "VertexLit"
}
