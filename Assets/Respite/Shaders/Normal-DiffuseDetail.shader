Shader "Diffuse Detail Unlit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Detail ("Detail (RGB)", 2D) = "gray" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 250
	
CGPROGRAM
#pragma surface surf NoLighting

sampler2D _MainTex;
sampler2D _Detail;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	float2 uv_Detail;
};

void surf (Input IN, inout SurfaceOutput o) 
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	c.rgb *= tex2D(_Detail,IN.uv_Detail).rgb*2;
	o.Albedo = c.rgb / 2.0;
	o.Alpha = c.a;
	

}

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
    {
        fixed4 c;
        c.rgb = s.Albedo;
        c.a = s.Alpha;
        return c;
    }

ENDCG
}

Fallback "Diffuse"
}
