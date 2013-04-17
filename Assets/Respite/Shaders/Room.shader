Shader "Custom/Room" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ShadowTex ("Shadow Pattern (RGB)", 2D) = "white" {}
		_Ambient("Ambient", Color) = (0.1, 1.0, 1.0, 1.0)
		_ShadowFactor("Shadow Factor", Range(0.0, 1.0)) = 0.5
		_FresnelIntensity("Fresnel Intensity", Range(0.0, 1.0)) = 0.1
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			sampler2D _ShadowTex;
			float4 _Progress;
			float4 _MainTex_ST;
			float4 _ShadowTex_ST;
			float _ShadowFactor;
			float _FresnelIntensity;
			float4 _Ambient;
			float4x4 _uvMatrix;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
		      float3 viewDir : TEXCOORD2;
		      float3 norm : TEXCOORD3;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color.xyz = _Progress.rgb;
		        o.color.w = 1.0;
		        
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _MainTex);
		        o.uv1 = TRANSFORM_TEX (v.texcoord1, _ShadowTex);
		        o.viewDir = WorldSpaceViewDir(v.vertex);
				o.norm = v.normal;
				
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				float4 val = tex2D(_MainTex, i.uv0);
				
				float4 otherval = tex2D(_ShadowTex, i.uv1);
				
				val = (val * _ShadowFactor) + ((1.0f - _ShadowFactor) * otherval);
				
				//val *= _TOD;
				
				val *= _Ambient;
				return val;
			}
			
			ENDCG
		}
	} 
}
