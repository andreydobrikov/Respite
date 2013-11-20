Shader "Custom/Sprite" 
{
	Properties 
	{
		_SpriteTex0 ("Sprite 0", 2D) = "white" {}
		_SpriteTex1 ("Sprite 1", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	 
			sampler2D _SpriteTex0;
			float4 _SpriteTex0_ST;
			sampler2D _SpriteTex1;
			float4 _SpriteTex1_ST;
			float4 _Color;
			float4x4 _Rotation;
			float _BlendFrameLerp;
			 
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
		        
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _SpriteTex0);
		        o.uv1 = TRANSFORM_TEX (v.texcoord, _SpriteTex1);
		        
		        return o; 
			}
			
			float4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 val = tex2D (_SpriteTex0, i.uv0) * _Color;
				float4 valAlt = tex2D (_SpriteTex1, i.uv1) * _Color;
				val *= i.color;
				return lerp(val, valAlt, _BlendFrameLerp);
			}
			
			ENDCG
		}
	} 
}
