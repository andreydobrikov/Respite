Shader "Custom/weather_mask_subtractive" 
{
Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags {"Queue" = "Transparent+10" }
 
 		cull off
		ZWrite Off
		BlendOp RevSub
		Blend One One
 
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct v2f 
			{
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			v2f vert (appdata_full v) 
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				// Assumes the value is grayscale
				float4 tex = tex2D (_MainTex, i.uv);
				float4 color;
				color.r = tex.r;
				color.gb = 0.0;
				color.a = 1.0;//tex.a;
			    return color;
			}
			ENDCG
		}
	}
}
