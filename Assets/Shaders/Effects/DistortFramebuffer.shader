Shader "Custom/DistortFramebuffer" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
		_RipplePower("Ripple Power", Range(0.0, 20.0)) = 8.0
		_TintIntensity("Tint Intensity", Range(0.0, 0.1)) = 0.05
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		
		GrabPass { "_FrameBuffer" }
				
		Pass
		{
		
			ZTest Always 
			Cull Off 
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _FrameBuffer;
			float4 _FrameBuffer_TexelSize;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _MainTex_ST;
			float4 _Color;
			float _RippleIntensity;
			float _RipplePower;
			float _TintIntensity;
			 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv : TEXCOORD0;
		      float4 grabUV : TEXCOORD1;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.color = v.color;
		        
		        #if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
		        
		        o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		        o.grabUV.xy = (float2(o.pos.x, scale * o.pos.y) + o.pos.w) * 0.5;
				o.grabUV.zw = o.pos.zw;
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				float2 normal = UnpackNormal(tex2D (_MainTex, i.uv)).rg;
				float2 offset = normal.xy * _MainTex_TexelSize.xy * _RipplePower * _RippleIntensity;// * 100.0f;
				i.grabUV.xy = offset + i.grabUV.xy;
				float4 frameBuffer = tex2D(_FrameBuffer, i.grabUV.xy);
				
				//frameBuffer = half4(abs(normal.x), abs(normal.y), 0.0f, 1.0f);
				//frameBuffer = half4(abs(offset.x), abs(offset.y), 0.0f, 1.0f);
				//float4 frameBuffer = half4(abs(i.grabUV.x), abs(i.grabUV.y), 0.0f, 1.0f);
			//	frameBuffer = tex2D (_MainTex, i.uv);
			frameBuffer.rgb += length(normal) * _RippleIntensity * _TintIntensity;
			frameBuffer.a = length(normal);
				//frameBuffer = half4(_RippleIntensity, 0.0f, 0.0f, 0.5f);
				return frameBuffer;
			}
			
			ENDCG
		}
	} 
}
