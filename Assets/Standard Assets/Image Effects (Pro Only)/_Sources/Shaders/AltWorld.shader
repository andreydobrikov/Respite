Shader "Hidden/AltWorld" 
{
	Properties 
	{
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Light Map", 2D) = "" {}
		_View ("View Camera", 2D) = "" {}
		_Color("Tint", Color) = (0.1, 0.8, 0.3, 1.0)
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;

	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	sampler2D _View;
	float4 _Color;
	
	
	half _Intensity;
	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v )
	 { 
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
	


	half4 frag (v2f i) : COLOR 
	{
	
		half4 altColour = tex2D(_Overlay, i.uv[0]);
		half4 view 		= tex2D(_View, i.uv[0]);
		half4 main 		= tex2D(_MainTex, i.uv[1]);
		//return half4(1.0f, 0.0f, 0.0f, 1.0f);
		
		//test.r = 0.0f;
		altColour.a *= 1.0f - view.r;

		half4 outColour = lerp(altColour, main, 1.0f - altColour.a);
		
		return  outColour;// * ((1.0f - outColour) * sum);//( * half4(0.6f, 0.6f, 0.6f, 0.4f));
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