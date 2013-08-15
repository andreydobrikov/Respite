Shader "Custom/TexTintDetail" 
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
		Cull off
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
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				const half4 white = half4(1.0, 1.0, 1.0, 1.0);
				float4 detailTex = tex2D(_DetailTex, i.uv1);
				
				float4 detail = lerp(detailTex, white, _DetailIntensity);
				float4 val = tex2D(_MainTex, i.uv0) * detail;
				
				val *= i.color;
				
				return val;
			}
			
			ENDCG
		}
	} 
}
