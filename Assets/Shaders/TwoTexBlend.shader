Shader "Custom/TwoTexBlendTransparent" 
{
	Properties 
	{
		_Source ("Source Tex", 2D) = "white" {}
		_SourceDetail ("Source Detail Tex", 2D) = "white" {}
		_Target ("Target Tex", 2D) = "white" {}
		_Blend ("Blend Tex", 2D) = "white" {}
		_DetailIntensity("Detail Intensity", Range(0.0, 1.0)) = 0.5
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
	
			sampler2D _Source;
			sampler2D _SourceDetail;
			sampler2D _Target;
			sampler2D _Blend;
			float4 _Source_ST;
			float4 _Target_ST;
			float4 _Blend_ST;
			float _DetailIntensity;
						 
			struct v2f 
			{
	          float4 pos : SV_POSITION;
	          fixed4 color : COLOR;
		      float2  uv0 : TEXCOORD0;
		      float2  uv1 : TEXCOORD1;
		      float2  uv2 : TEXCOORD2;
	        };
	
			v2f vert (appdata_full v) 
			{
				v2f o;
		        o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		        o.uv0 = TRANSFORM_TEX (v.texcoord, _Source);
		        o.uv1 = TRANSFORM_TEX (v.texcoord, _Target);
		        o.uv2 = TRANSFORM_TEX (v.texcoord1, _Blend);
		        o.color = v.color;
		        
		        return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{ 
				const half4 white = half4(1.0, 1.0, 1.0, 1.0);
			
				float4 source = tex2D(_Source, i.uv0);
				float4 target = tex2D(_Target, i.uv1);
				float4 sourceDetail = lerp(tex2D(_SourceDetail, i.uv2), white, _DetailIntensity);
				
				source = source * sourceDetail;
				
				float4 val = lerp(source, target, tex2D(_Blend, i.uv2).r);
				val.a = i.color.a;
				
				return val;
			}
			
			ENDCG
		}
	} 
}
