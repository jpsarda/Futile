Shader "Futile/Glow"
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_GlowColor ("Glow Color", Color) = (1,1,0,0)
		_GlowAmount ("Glow Amount", Range(0,02)) = 0.005
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
				//SetTexture [_MainTex] 
				//{
				//	Combine texture * primary
				//}
				
				
				
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma profileoption NumTemps=64
float4 _Color;
float4 _GlowColor;
sampler2D _MainTex;
float _GlowAmount;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    return o;
}

half4 frag (v2f i) : COLOR
{

    //half4 texcol = tex2D (_MainTex, i.uv);
    //return texcol * _Color;
    
    half4 texcol = tex2D(_MainTex, float2(i.uv.x, i.uv.y));
    
    if ( texcol.a<0.5 ) {
	    texcol = half4(0.0);
	    float remaining=1.0f;
	    float coef=1.0;
	    float fI=0;
	    for (int j = 0; j < 2; j++) {
	    	fI++;
	    	coef*=0.32;
	    	texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y - fI * _GlowAmount)) * coef;
	    	texcol += tex2D(_MainTex, float2(i.uv.x - fI * _GlowAmount, i.uv.y)) * coef;
	    	texcol += tex2D(_MainTex, float2(i.uv.x + fI * _GlowAmount, i.uv.y)) * coef;
	    	texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y + fI * _GlowAmount)) * coef;
	    	
	    	remaining-=4*coef;
	    }
	    texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * remaining;
	    //texcol.r+=1;
	    //texcol +=_GlowColor;
	    texcol+=_GlowColor;
    }
    
    //texcol.r=1;

    return texcol;
}
ENDCG
				
				
				
			}
		} 
	}
}
