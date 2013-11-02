Shader "Custom/Light" 
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
		Blend SrcAlpha One
		Cull Off
		
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
		        
		        
		        //o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
		        
		        half4 test = v.texcoord - half4(0.5f, 0.5f, 0.0f, 0.0f);

           		o.uv = mul ( test, _Rotation ).xy;
           		
           		o.uv += 0.5f;
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				
				
				float4 val = tex2D (_MainTex, i.uv) * _Color;
				val *= i.color;
				
				return val;
			}
			
			ENDCG
		}
	} 
}
