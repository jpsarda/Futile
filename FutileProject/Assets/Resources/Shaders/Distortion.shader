Shader "Custom/Distortion" {
	Properties {
		_MainTex ("", 2D) = "white" {}
		_CenterX ("CenterX", Range(-1,2)) = 0.5
		_CenterY ("CenterY", Range(-1,2)) = 0.5
		_Radius ("Radius", Range(-1,1)) = 0.2
		//_Rotation ("Rotation", Range(-1000000000,1000000000)) = 0.5
		_Amplitude ("Amplitude", Range(-10,10)) = 0.005
	}
	 
	SubShader {
	 
		ZTest Always Cull Off ZWrite Off Fog { Mode Off } //Rendering settings
	 
	 	Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//we include "UnityCG.cginc" to use the appdata_img struct
			
			float _CenterX;
			float _CenterY;
			float _Radius;
			//float _Rotation;
			float _Amplitude;

			struct v2f {
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};
	   
			//Our Vertex Shader
			v2f vert (appdata_img v){
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
				return o;
			}

			sampler2D _MainTex; //Reference in Pass is necessary to let us use this variable in shaders


			//Our Fragment Shader
			fixed4 frag (v2f i) : COLOR{
				//vec2 center=vec2(0.5,0.5);
				float2 diff=float2(i.uv.x-_CenterX,i.uv.y-_CenterY);
				float dist=sqrt(diff.x*diff.x+diff.y*diff.y);
				float2 ortho=float2(-diff.y,diff.x);
				
				//vec2 uv_displaced = vec2(i.uv.x + cos(i.uv.x*3.141592654)*0.05,i.uv.y + cos(i.uv.y*3.141592654)*0.05);
				//vec2 uv_displaced = vec2(i.uv.x + diff.x*cos(dist)*_Amplitude/dist,i.uv.y + diff.y*cos(dist)*_Amplitude/dist);
				//vec2 uv_displaced = vec2(i.uv.x - cos(dist*50)*diff.x*_Amplitude/dist,i.uv.y - cos(dist*50)*diff.y*_Amplitude/dist);
				//vec2 uv_displaced = vec2(i.uv.x + ortho.x*cos(dist)*_Amplitude/dist,i.uv.y + ortho.y*cos(dist)*_Amplitude/dist);
				//float ratio=cos(dist)*_Amplitude/dist;
				float ratio=sin(dist*8)*_Amplitude/dist;
				float2 uv_displaced = float2(i.uv.x + ortho.x*ratio,i.uv.y + ortho.y*ratio);

				fixed4 orgCol = tex2D(_MainTex, uv_displaced); //Get the orginal rendered color
				return orgCol;

				//fixed4 orgCol = tex2D(_MainTex, i.uv); //Get the orginal rendered color

				//Make changes on the color
				//float avg = (orgCol.r + orgCol.g + orgCol.b)/4f;
				//fixed4 col = fixed4(orgCol.r*0.66, orgCol.g*0.66, orgCol.b*1, 1);

				//return col;
			}
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}