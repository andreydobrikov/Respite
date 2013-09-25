Shader "Custom/RenderDepthBuffer" 
{
	Properties {}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float4 projPos : TEXCOORD0;
	};
			
	uniform sampler2D _CameraDepthTexture; //the depth texture
	
	v2f vert( appdata_img v ) 
	{ 
		v2f o;
		o.pos 		= mul(UNITY_MATRIX_MVP, v.vertex);
		o.projPos 	= ComputeScreenPos(o.pos); 
		return o;
	}
	


	float4 frag (v2f i) : COLOR 
	{
		float depth = Linear01Depth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
		
		return float4(depth, depth, depth, 1.0f);
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