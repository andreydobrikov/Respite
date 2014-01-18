Shader "Hidden/ViewRegion" {
	Properties
	 {
	  	_MainTex ("", any) = "" {} 
	  	_SourceTex("Source", 2D) = "" {}
	  	_BlendTex ("Blur", 2D) = "" {}
	 }
	
	/*SubShader { 
		Pass {
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }
			SetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant alpha}
			SetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}
			SetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}
			SetTexture [_MainTex] {constantColor (0,0,0,0.25) combine texture * constant + previous}
		}
	}*/
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : POSITION;
		half2 uvMain : TEXCOORD0; 
		half2 uv : TEXCOORD1;
		half2 taps[4] : TEXCOORD2; 
	};
	sampler2D _MainTex;
	sampler2D _SourceTex;
	sampler2D _BlendTex;
	half4 _MainTex_TexelSize;
	half4 _BlurOffsets;
	
	v2f vert( appdata_img v ) 
	{
		v2f o; 
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord - _BlurOffsets.xy * _MainTex_TexelSize.xy; // hack, see BlurEffect.cs for the reason for this. let's make a new blur effect soon
		o.uvMain = v.texcoord;
		o.taps[0] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[1] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy;
		o.taps[2] = o.uv + _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
		o.taps[3] = o.uv - _MainTex_TexelSize * _BlurOffsets.xy * half2(1,-1);
		
		return o;
	}
	
	half4 frag(v2f i) : COLOR 
	{
		half4 mainTex = tex2D(_SourceTex, i.uvMain);
		half4 blend = tex2D(_BlendTex, i.uvMain);
		half4 color = tex2D(_MainTex, i.taps[0]);
		color += tex2D(_MainTex, i.taps[1]);
		color += tex2D(_MainTex, i.taps[2]);
		color += tex2D(_MainTex, i.taps[3]); 
		color *= 0.25;
		
		float lerpFactor = ( blend.g);
		
		float intensity =  0.3f * color.r + 0.59f * color.g + 0.11f * color.b;
		intensity *= 0.8f;
		//intensity *= 0.15f;
		color = lerp(half4(intensity, intensity, intensity, 1.0 ), color , lerpFactor * 0.3f);
		
		color = lerp(color, mainTex, lerpFactor);
		
		return color;//mainTex;//half4(1.0, 0.0, 0.0 ,1.0);
	}
	
	ENDCG
	SubShader {
		 Pass {
			  ZTest Always Cull Off ZWrite Off
			  Fog { Mode off }      

			  CGPROGRAM
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment frag
			  ENDCG
		  }
	}
	Fallback off
}