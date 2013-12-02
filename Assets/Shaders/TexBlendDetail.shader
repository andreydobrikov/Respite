Shader "Custom/TexBlendDetail" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_BaseDetailTex ("Base Detail Tex", 2D) = "white" {}
		_DetailTex ("Detail Tex", 2D) = "white" {}
		_Blend ("Blend Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
		_DetailIntensity("Detail Intensity", Range(0.0, 1.0)) = 0.5
		_BaseDetailIntensity("Base Detail Intensity", Range(0.0, 1.0)) = 0.5
	}
	
	SubShader 
	{
		Tags {"Queue"="Geometry"}
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _BaseDetailTex;
			sampler2D _DetailTex;
			sampler2D _Blend;
			float4 _MainTex_ST;
			float4 _DetailTex_ST;
			float4 _Blend_ST;
			float4 _Color;
			float _DetailIntensity;
			float _BaseDetailIntensity;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
		      float2  uv2 : TEXCOORD2;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		        
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);
		        o.uv1 = TRANSFORM_TEX (v.texcoord1, _DetailTex);
		        o.uv2 = TRANSFORM_TEX (v.texcoord1, _Blend);
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				const half4 white = half4(1.0, 1.0, 1.0, 1.0);
				float4 baseDetailTex = tex2D(_BaseDetailTex, i.uv2);
				float4 detailTex = tex2D(_DetailTex, i.uv1);
				float4 blendTex = tex2D(_Blend, i.uv2);
				
				detailTex = detailTex * blendTex;
				detailTex.a = 0.0;
				
				float4 detail = length(detailTex) * _DetailIntensity;
				
				
				float4 val = tex2D(_MainTex, i.uv0) * (1.0f - length(detail)) * (1.0f - ((1.0f - baseDetailTex.r) * _BaseDetailIntensity));
				
				val *= i.color;
				//val = length(detailTex);
				return val;
			}
			
			ENDCG
		}
	} 
}
