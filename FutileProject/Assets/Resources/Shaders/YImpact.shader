Shader "Custom/YImpact" {
	Properties {
		_MainTex ("", 2D) = "white" {}
		_CenterX ("CenterX", Range(-1,2)) = 0.5
		_CenterY ("CenterY", Range(-1,2)) = 0.5
		_Radius ("Radius", Range(-1,1)) = 0.2
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
				float diff=i.uv.y-_CenterY;
				float dist=abs(diff);
				float2 uv_displaced = float2(i.uv.x,i.uv.y + diff*cos(dist*3)*_Amplitude/dist);
				
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