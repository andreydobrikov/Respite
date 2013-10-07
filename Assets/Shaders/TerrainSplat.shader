Shader "Custom/TerrainSplat" 
{
	Properties 
	{
		_Layer0 ("Layer 0 Tex", 2D) = "white" {}
		_Layer1 ("Layer 1 Tex", 2D) = "white" {}
		_Layer2 ("Layer 2 Tex", 2D) = "white" {}
		_Detail ("Detail Tex", 2D) = "white" {}
		_Splat ("Splat Tex", 2D) = "white" {}
		_DetailIntensity("Detail Intensity", Range(0.0, 1.0)) = 0.5
	}
	
	SubShader 
	{
	Tags { "RenderType"="Opaque" }
		Cull off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _Layer0;
			sampler2D _Layer1;
			sampler2D _Layer2;
			sampler2D _Detail;
			sampler2D _Splat;
			float4 _Layer0_ST;
			float4 _Layer1_ST;
			float4 _Layer2_ST;
			float4 _Detail_ST;
			float4 _Splat_ST;
			float _DetailIntensity;
			
			float4 _SplatDisplayOverride;
			float _SplatDisplayFactor;
						 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
		      float2  uv2 : TEXCOORD2;
		      float2  uv3 : TEXCOORD3;
		      float2  uv4 : TEXCOORD4;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _Layer0);
		        o.uv1 = TRANSFORM_TEX (v.texcoord, _Layer1);
		        o.uv2 = TRANSFORM_TEX (v.texcoord, _Layer2);
		        o.uv3 = TRANSFORM_TEX (v.texcoord, _Detail);
		        o.uv4 = TRANSFORM_TEX (v.texcoord, _Splat);
		        o.color = v.color;
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				const half4 white = half4(1.0, 1.0, 1.0, 1.0);
			
				// Grab dem textures
				float4 sourceDetail = tex2D(_Detail, i.uv3); 
				float4 splat 		= tex2D(_Splat, i.uv4);
				float4 layer0 		= tex2D(_Layer0, i.uv0);
				float4 layer1 		= tex2D(_Layer1, i.uv1);
				float4 layer2		= tex2D(_Layer2, i.uv2);
				
				// Modulate the detail texture by the splat-map. 
				float3 detail 	= (sourceDetail * splat);
				
				// The detail ends up just being an intensity.
				// The splat map rgb should always be a unit vector, so summing the components of the modulated detail gives the intensity.
				// Detail darkens, so do a "1.0f - x";
				detail 			= float3(1.0f - ((detail.r + detail.g + detail.b) ));
				
				detail = lerp(detail, white, 1.0f - ( _DetailIntensity * splat.a));
				
				float3 combined = ((layer0.rgb * splat.r) + (layer1.rgb * splat.g) + (layer2.rgb * splat.b)) ;
				
				combined.rgb *= detail;
				
				float4 val = white;
				val.rgb = combined;
				val.a = i.color.a;
				
				val.rgb = lerp(val.rgb, (splat.rgb * _SplatDisplayOverride.rgb), _SplatDisplayFactor);
				
				return val;
			}
			
			ENDCG
		}
	} 
}
