Shader "Hidden/ViewRegion" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Light Map", 2D) = "" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	
	half _Intensity;
	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv[0] =  v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	
		return o;
	}
	


	half4 frag (v2f i) : COLOR {
		half4 toBlend = tex2D(_Overlay, i.uv[0]);
		toBlend.a = 1.0f;
		//return half4(1.0f, 0.0f, 0.0f, 1.0f);
		//half4 test = tex2D(_MainTex, i.uv[1]);
		//test.r = 0.0f;
		return  tex2D(_MainTex, i.uv[1]) * (1.0f - (toBlend ));
	}	

	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  
      ColorMask RGB	  
  		  	
	 Pass {    
	
	      CGPROGRAM
	      #pragma fragmentoption ARB_precision_hint_fastest
	      #pragma vertex vert
	      #pragma fragment frag
	      ENDCG
	  }
  
}

Fallback off
	
} // shader