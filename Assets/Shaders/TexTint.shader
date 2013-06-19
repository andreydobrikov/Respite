Shader "Custom/TexTint" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
	}
	
	SubShader 
	{
		LOD 200
		
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv : TEXCOORD0;
	        };
	
			v2f vert (appdata_base v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = _Color;
		        
		        
		        o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 val = tex2D (_MainTex, i.uv) * _Color;
				return val;
			}
			
			ENDCG
		}
	} 
}
