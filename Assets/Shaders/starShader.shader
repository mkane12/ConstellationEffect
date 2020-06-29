Shader "Custom/starShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_CurrTex ("Albedo", 2D) = "white" {}
		_NextTex ("Albedo", 2D) = "white" {}
		_Transparency("Transparency", Float) = 0.9
		_Blend("Blend", Float) = 0.5
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
		LOD 200

		// tells us not to render to Depth Buffer
		// > drawing semi-transparent object, so switch to off
		ZWrite Off
		Lighting Off

		// blend using alpha channel
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata {
				// black magic universally understood by all operating systems
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _CurrTex;
			float4 _CurrTex_ST;
			sampler2D _NextTex;
			float4 _NextTex_ST;
			float4 _Color;
			float _Transparency;
			float _Blend;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target { // called once for each pixel
				// TRANSFORM_TEX scales and offsets texture coordinates; transforms 2D UV by scale/bias property
				float2 uvCurr = TRANSFORM_TEX(i.texcoord, _CurrTex);
				float2 uvNext = TRANSFORM_TEX(i.texcoord, _NextTex);

				float3 currColor = tex2D(_CurrTex, uvCurr) * _Color;
				float3 nextColor = tex2D(_NextTex, uvNext) * _Color;
				float3 input_color = lerp(currColor, nextColor, _Blend);
				// make transparent when texture is black (background)
				float pct = smoothstep(0, 1, (input_color.r + input_color.g + input_color.b)*2/3);
				float trans = lerp(0, _Transparency, pct);
				return float4(input_color, max(trans, 0));
			}

			ENDCG
		}
		
	}
	FallBack "Diffuse"
}
