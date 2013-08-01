Shader "Hidden/ViewRegion" 
{
	Properties 
	{
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Light Map", 2D) = "" {}
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
	
			half4 toBlend = tex2D(_Overlay, i.uv[0]);
		toBlend.a = 1.0f;
		//return half4(1.0f, 0.0f, 0.0f, 1.0f);
		//half4 test = tex2D(_MainTex, i.uv[1]);
		//test.r = 0.0f;
		half4 outColour = (1.0f - (toBlend ));
		
		
		float blurAmount = 0.0075;
 
          half4 sum = half4(0.0);
 
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y - 5.0 * blurAmount)) * 0.025;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y - 4.0 * blurAmount)) * 0.05;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y - 3.0 * blurAmount)) * 0.09;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y - 2.0 * blurAmount)) * 0.12;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y - blurAmount)) * 0.15;
       /* sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y)) * 0.16;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y + blurAmount)) * 0.15;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y + 2.0 * blurAmount)) * 0.12;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y + 3.0 * blurAmount)) * 0.09;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y + 4.0 * blurAmount)) * 0.05;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[1].y + 5.0 * blurAmount)) * 0.025;
        
        sum += tex2D(_MainTex, float2(i.uv[0].x - 5.0 * blurAmount, i.uv[0].y)) * 0.025;
        sum += tex2D(_MainTex, float2(i.uv[0].x - 4.0 * blurAmount, i.uv[0].y)) * 0.05;
        sum += tex2D(_MainTex, float2(i.uv[0].x - 3.0 * blurAmount, i.uv[0].y)) * 0.09;
        sum += tex2D(_MainTex, float2(i.uv[0].x - 2.0 * blurAmount, i.uv[0].y)) * 0.12;
        sum += tex2D(_MainTex, float2(i.uv[0].x - blurAmount, i.uv[0].y)) * 0.15;
        sum += tex2D(_MainTex, float2(i.uv[0].x, i.uv[0].y)) * 0.16;
        sum += tex2D(_MainTex, float2(i.uv[0].x + blurAmount, i.uv[0].y)) * 0.15;
        sum += tex2D(_MainTex, float2(i.uv[0].x + 2.0 * blurAmount, i.uv[0].y)) * 0.12;
        sum += tex2D(_MainTex, float2(i.uv[0].x + 3.0 * blurAmount, i.uv[0].y)) * 0.09;
        sum += tex2D(_MainTex, float2(i.uv[0].x + 4.0 * blurAmount, i.uv[0].y)) * 0.05;
        sum += tex2D(_MainTex, float2(i.uv[0].x + 5.0 * blurAmount, i.uv[0].y)) * 0.025; 
 */
		
		return tex2D(_MainTex, i.uv[1]) * outColour;// * ((1.0f - outColour) * sum);//( * half4(0.6f, 0.6f, 0.6f, 0.4f));
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