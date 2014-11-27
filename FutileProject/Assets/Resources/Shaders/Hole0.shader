Shader "Futile/Hole0"
{
	Properties 
	{
		_Hole0Tex ("Hole (RGB) Trans (A)", 2D) = "white" {}
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	
	Category 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque"}
		ZWrite Off
		//Alphatest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha
		Fog { Color(0,0,0,0) }
		Lighting Off
		Cull Off //we can turn backface culling off because we know nothing will be facing backwards

		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "texcoord", texcoord 
			Bind "Color", color 
		}

		SubShader   
		{
			Pass 
			{
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma profileoption NumTemps=64

float4 _Color;
sampler2D _MainTex;
sampler2D _Hole0Tex;


struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
    float2 hole0uv : TEXCOORD1;
};

float4 _MainTex_ST;
float4 _Hole0Tex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    o.hole0uv = TRANSFORM_TEX (v.texcoord, _Hole0Tex);
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv) * _Color;
    texcol.a *= (1-tex2D(_Hole0Tex, i.hole0uv).a) ;
    return texcol;
}
ENDCG
		
		
			}
		} 
	}
}
