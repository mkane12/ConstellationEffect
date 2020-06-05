Shader "Custom/starShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Transparency("Transparency", Float) = 0.9
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

			sampler2D _MainTex;
			// TODO: What does this MainTex_ST do?? It's not explicitly called anywhere...
			float4 _MainTex_ST;
			float4 _Color;
			float _Transparency;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target { // called once for each pixel
				float2 uv = TRANSFORM_TEX(i.texcoord, _MainTex);
				float3 input_color = tex2D(_MainTex, uv) * _Color;
				// fade at edges
				float t = length(i.texcoord - float2(0.5, 0.5)) * 1.41421356237;
				float trans = lerp(_Transparency, 0, t + 0.25);
				return float4(input_color, max(trans, 0));
			}

			ENDCG
		}
		
	}
	FallBack "Diffuse"
}
