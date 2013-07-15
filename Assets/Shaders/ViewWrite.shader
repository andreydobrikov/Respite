Shader "Custom/ViewWrite" 
{
	SubShader {
		// Render the mask after regular geometry, but before masked geometry and
		// transparent things.
 
		Tags {"Queue" = "Transparent+10" }
 
		// Don't draw in the RGBA channels; just the depth buffer
 		cull off
		ZWrite Off
 
		// Do nothing specific in the pass:
 
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;
			
			
			struct v2f {
			    float4  pos : SV_POSITION;
			};
			
			v2f vert (appdata_full v) 
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    return half4(0.0f, 0.0f, 0.0f, 0.0f);
			}
			ENDCG
		}
	}
}
