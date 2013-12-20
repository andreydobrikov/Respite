Shader "Custom/LightDetail" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_DetailTex ("Detail Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
		_DetailIntensity("Detail Intensity", Range(0.0, 1.0)) = 0.5
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha One
		Cull Off
		
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _DetailTex;
			float4 _MainTex_ST;
			float4 _DetailTex_ST;
			float4 _Color;
			float _DetailIntensity;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		        
		        
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);
		        o.uv1 = TRANSFORM_TEX (v.texcoord1, _DetailTex);
		        
		        return o;
			}
			
			float4 frag (v2f i) : COLOR0 
			{ 
				const half4 white = half4(1.0, 1.0, 1.0, 1.0);
				float4 detail = lerp(white, tex2D(_DetailTex, i.uv1) , _DetailIntensity);
				float4 val = tex2D(_MainTex, i.uv0) * detail;
				
				val *= i.color;
				val *= _Color;
				
				//val = half4(i.uv1.x, i.uv1.y, 0.0, 1.0);
				return val;
			}
			
			ENDCG
		}
	} 
}
