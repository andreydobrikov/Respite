Shader "Custom/SnowOverlay" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_MaskTex("Mask Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
			Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
		LOD 200
		ZTest Always
		//Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float4 _Color;
			float4x4 _Rotation;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv : TEXCOORD0;
		      float4 screenPos : TEXCOORD1;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		         
		        
		        //o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		        
		        half4 test = v.texcoord - half4(0.5f, 0.5f, 0.0f, 0.0f);

				half2 ffs = mul ( test, _Rotation ).xy;
           		ffs += 0.5f;
           		
           		o.uv = TRANSFORM_TEX(ffs, _MainTex);
           		o.screenPos = mul(UNITY_MATRIX_MVP, v.vertex);
    			o.screenPos.xy /= o.screenPos.w;
   			 o.screenPos.xy = 0.5*(o.screenPos.xy+1.0);
		        
		        return o; 
			}
			
			float4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 val = tex2D (_MainTex, i.uv); 
				val.a *= _Color.a;
				float4 mask = tex2D(_MaskTex, i.screenPos.xy);
				val *= i.color;
				val.a *= (1.0 - mask.r);
				return val;
			}
			
			ENDCG
		}
	} 
}
