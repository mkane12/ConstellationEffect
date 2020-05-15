﻿Shader "Custom/starShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Transparency("Transparency", Float) = 1.0
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
		LOD 200

		// tells us not to render to Depth Buffer
		// > drawing semi-transparent object, so switch to off
		ZWrite Off

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
			float4 _Color;
			float _Transparency;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target { // called once for each pixel
				// sample texture
				fixed4 col = tex2D(_MainTex, i.texcoord) + _Color;
				col.a = _Transparency;
				return col;
			}

			ENDCG
		}


		/*Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}

		LOD 200

		Pass {
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			fixed4 _Color;

			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		}*/
		
	}
	FallBack "Diffuse"
}
