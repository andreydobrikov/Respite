Shader "Custom/Tree" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AltTex ("Alt (RGB)", 2D) = "white" {}
		_ShadowTex ("Shadow Pattern (RGB)", 2D) = "white" {}
		_HighlightTex ("Highlight Pattern (RGB)", 2D) = "white" {}
		
		_BaseColour("Base Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_ShadeProgress("Shade Progress", Color) = (0.1, 1.0, 1.0, 1.0)
		_ShadowIntensity("Shadow Intensity", Range(0.0, 1.0)) = 0.5
		_HighlightIntensity("Highlight Intensity", Range(0.0, 1.0)) = 0.1
	}
	
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			sampler2D _AltTex;
			sampler2D _ShadowTex;
			sampler2D _HighlightTex;
			
			float4 _BaseColour;
			float4 _ShadeProgress;
			float _ShadowIntensity;
			float _HighlightIntensity;
			
			float4 _MainTex_ST;
			float4 _ShadowTex_ST;
			float4x4 _uvMatrix;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
		      float3 viewDir : TEXCOORD2;
		      float3 norm : TEXCOORD3;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);
		        o.uv1 = TRANSFORM_TEX (v.texcoord1, _ShadowTex);
		        o.viewDir = WorldSpaceViewDir(v.vertex);
				o.norm = v.normal;
				
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				float4 val = tex2D(_MainTex, i.uv0);
				float4 alt = tex2D(_AltTex, i.uv0);
				
				val = (_ShadeProgress.r * val) + ((1.0f - _ShadeProgress.r) * alt);
				
				float4 highlightTex = tex2D(_HighlightTex, i.uv1);
				float4 shadowTex = tex2D(_ShadowTex, i.uv1);
				
				
				val.rgb *= _BaseColour;
				
				val.rgb += (length((highlightTex.rgb * _ShadeProgress) * highlightTex.a) * _HighlightIntensity) * float3(0.9, 0.9, 0.7);
				//val = normalize(val);
				
				val.rgb -= (length((shadowTex.rgb * _ShadeProgress) * shadowTex.a) * _ShadowIntensity);
				
				//val = normalize(val);
				
				//val *= _TOD;
				//val = otherVal;
				//val *= _Ambient;
				return val;
			}
			
			ENDCG
		}
	} 
}

