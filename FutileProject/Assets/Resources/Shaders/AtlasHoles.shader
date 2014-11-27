Shader "Futile/AtlasHoles"
{
	Properties 
	{

		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_Hole0OffU ("Hole0 offset u", Float) = 0
		_Hole1OffU ("Hole1 offset u", Float) = 0
		
		_Hole0OffV ("Hole0 offset v", Float) = 0
		_Hole1OffV ("Hole1 offset v", Float) = 0
		
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
				 // Apply base texture
	            //SetTexture [_MainTex] {
	            //    combine texture
	            //}
	            // Blend in the alpha texture using the lerp operator
	            //SetTexture [_Hole0Tex] {
	            //    combine texture lerp (texture) previous
	            //}
	            
	            
		
		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
//#include "AngryInclude.cginc"
#pragma profileoption NumTemps=64

float4 _Color;
sampler2D _MainTex;
float _Hole0OffU;
float _Hole1OffU;
float _Hole0OffV;
float _Hole1OffV;


struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_base v)
{
    v2f o;
    //o.pos = (1,-1,-1,-1); 
    //v.vertex.x-=100;
    //v.vertex.y-=100;
    //v.vertex.z+=400;
    o.pos=mul (UNITY_MATRIX_MVP, v.vertex);
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    return o;
}

float2 offsetUV;
half4 frag (v2f i) : COLOR
{
	//return (20,0,0,20);
    half4 texcol = tex2D (_MainTex, i.uv) * _Color;
    //half4 texcol = tex2D (_Hole0Tex, i.uv) * _Color;
    
    
    if (( _Hole0OffU != 0 ) || ( _Hole0OffV != 0 )) {
    	offsetUV=i.uv;
	    offsetUV.x+=_Hole0OffU;
	    offsetUV.y+=_Hole0OffV;
    	texcol.a *= (1-tex2D(_MainTex, offsetUV).a) ;
    }
    
    if (( _Hole1OffU != 0 ) || ( _Hole1OffV != 0 )) {
    	offsetUV=i.uv;
	    offsetUV.x+=_Hole1OffU;
	    offsetUV.y+=_Hole1OffV;
    	texcol.a *= (1-tex2D(_MainTex, offsetUV).a) ;
    }
    //texcol.a=0.5;
    //texcol.r=1;
    return texcol;
}
ENDCG
		
		
			}
		} 
	}
}
