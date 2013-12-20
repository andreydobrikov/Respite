Shader "Custom/WeatherMask" 
{
	SubShader 
	{
		Tags {"Queue" = "Transparent+10" }
 
 		cull off
		ZWrite Off
		BlendOp Add
		Blend One One
 
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct v2f 
			{
			    float4  pos : SV_POSITION;
			};
			
			v2f vert (appdata_full v) 
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    return half4(1.0f, 0.0f, 0.0f, 1.0f);
			}
			ENDCG
		}
	}
}
