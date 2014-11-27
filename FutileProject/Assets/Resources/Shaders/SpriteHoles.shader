
Shader "Futile/SpriteHoles"
{
	Properties 
	{
		
		
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_Hole0Tex ("Hole (RGB) Trans (A)", 2D) = "white" {}
		_Hole0MatrixABCD ("Matrix part 1 : a b c d", Color) = (1,0,0,1)
		_Hole0MatrixTxTy ("Matrix part 2 : tx ty unused unsued", Color) = (0,0,0,0)
		_Hole0UVRect ("UVRect : uMin vMin uMax vMax", Color) = (0,0,0,0)
		//_Hole0LocalToUV ("Local to UV params factorX offsetX factorY offsetY", Color) = (0,0,0,0)
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
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members scrPos)
#pragma exclude_renderers d3d11 xbox360
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma profileoption NumTemps=64

float4 _Color;
sampler2D _MainTex;
sampler2D _Hole0Tex;
float4 _Hole0MatrixABCD;
float4 _Hole0MatrixTxTy;
float4 _Hole0UVRect;
//float4 _Hole0LocalToUV;

//x=offsetX
//y=offsetY
//z=displayScaleInverse
//w=unused
//uniform float4 _screenParams;
//uniform float4 _screenSize;

struct v2f {
    float4  pos : SV_POSITION;
    float2  uv : TEXCOORD0;
    float2 hole0uv : TEXCOORD1;
    //float4 scrPos;
    fixed4 vertexColor : COLOR;
};

float4 _MainTex_ST;
float4 _Hole0Tex_ST;
//float4 _stage;
//float4 _local;

v2f vert (appdata_full v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    //No need to use screen pos anymore, using v.vertex, much more performant
    //need to know what ComputeScreenPos does exactly, and try to integrate this in in _Hole0MatrixABCD and  _Hole0MatrixTxTy from the C# code 
    //o.scrPos = ComputeScreenPos(o.pos);
    //o.scrPos.x*=_screenSize.x;
	//o.scrPos.y*=_screenSize.y;
    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
    //TANSFORM_TEXT is same as this
    //o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    o.vertexColor=v.color;
    
    
    //futile stage sprite coords
    //old way
    //_stage.x=(o.scrPos.x*_screenSize.x+_screenParams.x)*_screenParams.z;
    //_stage.y=(o.scrPos.y*_screenSize.y+_screenParams.y)*_screenParams.z;
    //_local.x=_stage.x*_Hole0MatrixABCD.x + _stage.y*_Hole0MatrixABCD.z + _Hole0MatrixTxTy.x;
    //_local.y=_stage.x*_Hole0MatrixABCD.y + _stage.y*_Hole0MatrixABCD.w + _Hole0MatrixTxTy.y;
	//o.hole0uv.x=_local.x*_Hole0LocalToUV.x + _Hole0LocalToUV.y;
	//o.hole0uv.y=_local.y*_Hole0LocalToUV.z + _Hole0LocalToUV.w;
	
	//Using screen coordinates
	//o.hole0uv.x=o.scrPos.x*_Hole0MatrixABCD.x + o.scrPos.y*_Hole0MatrixABCD.z + _Hole0MatrixTxTy.x;
	//o.hole0uv.y=o.scrPos.x*_Hole0MatrixABCD.y + o.scrPos.y*_Hole0MatrixABCD.w + _Hole0MatrixTxTy.y;
	
	//Using directly vertex coodrinates, much better!
	//Maybe could use a matrixfor performances?
	o.hole0uv.x=v.vertex.x*_Hole0MatrixABCD.x + v.vertex.y*_Hole0MatrixABCD.z + _Hole0MatrixTxTy.x;
	o.hole0uv.y=v.vertex.x*_Hole0MatrixABCD.y + v.vertex.y*_Hole0MatrixABCD.w + _Hole0MatrixTxTy.y;
	
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 texcol = tex2D (_MainTex, i.uv) * _Color * i.vertexColor;
    
    //hole
    if ((i.hole0uv.x>_Hole0UVRect.x)&&(i.hole0uv.x<_Hole0UVRect.z)&&(i.hole0uv.y>_Hole0UVRect.y)&&(i.hole0uv.y<_Hole0UVRect.w)) {
    	texcol.a *= (1-tex2D(_Hole0Tex, i.hole0uv).a) ;
    }
    
    //mask (comment hole section and uncomment mask section to turn this shader into masking)
    //if ((i.hole0uv.x>_Hole0UVRect.x)&&(i.hole0uv.x<_Hole0UVRect.z)&&(i.hole0uv.y>_Hole0UVRect.y)&&(i.hole0uv.y<_Hole0UVRect.w)) {
    //	texcol.a *= tex2D(_Hole0Tex, i.hole0uv).a ;
    //} else {
    //	texcol.a=0;
    //}
    
    //tests
    //float2 wcoord = (i.scrPos.xy);
	//fixed4 color;
	//color = fixed4(wcoord.xy,0.0,1.0);
	//return color;
    
    //half4 texcol = tex2D (_Hole0Tex, i.hole0uv) * _Color;
    return texcol;
}
ENDCG
		
		
			}
		} 
	}
}
