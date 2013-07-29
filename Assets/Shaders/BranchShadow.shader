Shader "Custom/BranchShadow" 
{
	Properties 
	{
		_MainTex ("Diffuse Tex", 2D) = "white" {}
		_Target ("Target Tex", 2D) = "white" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
	}
	
	SubShader 
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			sampler2D _Target;
			float4 _MainTex_ST;
			float4 _Color;
			float _Progress;
			 
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
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				//float4 val = i.color + (1.0f - (tex2D (_ShadowTex, i.uv).r *_ShadowFactor));
				//half rim = 1.0 - saturate(dot (normalize(i.viewDir), i.norm));
				//rim = smoothstep(0.4, 1.0f, rim);
				
				//val.rgb += rim * _FresnelIntensity;//i.norm;
				//val.rgb = float3(1.0f, 0.0f, 0.0f);
				
				float4 blendVal = lerp(tex2D (_MainTex, i.uv), tex2D(_Target, i.uv), _Progress);
				
				//blendVal = tex2D(_Target, i.uv);
				
				float4 val = blendVal * _Color;
				val *= i.color;
				return val;
			}
			
			ENDCG
		}
	} 
}
