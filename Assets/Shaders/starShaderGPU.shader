Shader "Custom/starShaderGPU" {
	
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Transparency ("Transparency", Range(0,1)) = 1.0
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	SubShader{

	CGPROGRAM
	
	// compiler directive; instructs shader compiler to generate a surface shader with standard lighting and full support for shadows
	#pragma surface ConfigureSurface Standard fullforwardshadows addshadow

	// vertex and fragment shaders to render twinkling
	/*#pragma vertex vert
	#pragma fragment frag*/

	// indicate surface shader needs to invoke a ConfigureProcedural function per vertex
	#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural

	// turn off asynchronous shading so dummy shader isn't made when doing procedural drawing
	#pragma editor_sync_compilation


	// minimum for shader's target level and quality
	#pragma target 4.5

	// define input structure for configuration function
	struct Input {
		float3 worldPos;
		float2 uv_MainTex;
	};

	struct InstanceData {
		float3 startPosition; // start position (touch position)
		float3 position; // position of star on mesh
		float alpha; // star transparency
		float lifespan; // star's lifespan
		float timeToFade; // time over which star fades
		int twinkleIndex; // index on sprite sheet for twinkle animation
	};

	/*struct appdata {
		// black magic universally understood by all operating systems
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
	};*/

	float _Smoothness;
	float _Transparency;
	float _Size;
	float3 _Color;

	sampler2D _MainTex;


	/*v2f vert (appdata v) {
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
	}*/

	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		StructuredBuffer<float3> _Vertices;
		StructuredBuffer<InstanceData> _InstanceDataBuffer;
	#endif

	void ConfigureProcedural() 
	{
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			float3 position = _InstanceDataBuffer[unity_InstanceID].position;

			unity_ObjectToWorld = 0.0;
			unity_ObjectToWorld._m03_m13_m23_m33 = float4(position, 1.0); // this is star position
			unity_ObjectToWorld._m00_m11_m22 = _Size; // this is star scale
		#endif
	}

	// define ConfigureSurface method - result indicated by inout
	void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) 
	{
		#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
			float alpha = _InstanceDataBuffer[unity_InstanceID].alpha;
			_Transparency = alpha;
		#endif

		surface.Albedo = tex2D (_MainTex, input.uv_MainTex).rgb * (_Color, 1.0) + (0.0, 0.0, 0.0, _Transparency);
	}

	ENDCG
	
	}

	FallBack "Diffuse"
}
