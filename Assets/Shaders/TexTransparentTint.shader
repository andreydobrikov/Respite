Shader "Custom/TexTransparentTint" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
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
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4x4 _Rotation;
			 
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
		        
		        half4 test = v.texcoord - half4(0.5f, 0.5f, 0.0f, 0.0f);

				half2 ffs = mul ( test, _Rotation ).xy;
           		ffs += 0.5f;
           		
           		o.uv = TRANSFORM_TEX(ffs, _MainTex);
		        
		        return o; 
			}
			
			float4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 val = tex2D (_MainTex, i.uv) * _Color;
				val *= i.color;
				//val.r = abs(i.uv.x);
				//val.g = abs(i.uv.y);
				//val.b = 0.0;
				return val;
			}
			
			ENDCG
		}
	} 
}
