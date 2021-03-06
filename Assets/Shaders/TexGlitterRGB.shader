Shader "Custom/TexGlitterRGB" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_GlitterRGBTex ("Diffuse Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
		_Progress("Progress", Color) = (1.0, 0.0, 0.0, 1.0)
	}
	
	SubShader 
	{
		LOD 200
		Cull off
		
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			sampler2D _GlitterRGBTex;
			float4 _MainTex_ST;
			float4 _GlitterRGBTex_ST;
			float4 _Color;
			float4 _Progress;
			 
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
		        o.uv1 = TRANSFORM_TEX (v.texcoord1, _GlitterRGBTex);
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 val = tex2D (_MainTex, i.uv0) * _Color;
				float4 glitter = tex2D (_GlitterRGBTex, i.uv1);
				
				val *= i.color;
				val *= _Color;
				val.rgb += length(_Progress.rgb * glitter) * 0.1f;
				return val;
			}
			
			ENDCG
		}
	} 
}
