Shader "Custom/alphaEdgeDetect" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
		_Min("Min", Range(0.0, 1.0)) = 0.1
		_Max("Max", Range(0.0, 1.0)) = 0.8
		_AlphaMultiplier("Alpha falloff", Range(0.0, 1.0)) = 1.0
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Min;
			float _Max;
			float _AlphaMultiplier;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv : TEXCOORD0;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		        
		        
		        o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				float4 val = tex2D (_MainTex, i.uv) * _Color;
				
				half4 final = half4(1.0f, 0.0f, 0.0f,  _AlphaMultiplier);
				
				final.rgb = _Color.rgb;
				final.a = smoothstep(_Min, _Max, val.a) * _AlphaMultiplier;
				
				return final;
			}
			
			ENDCG
		}
	} 
}
