Shader "Custom/starShaderGPU" {
	
	// TODO: right now this is just a rainbow, so will need to switch to twinkling star sheet at some point

	Properties {
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_Transparency ("Transparency", Range(0,1)) = 1.0
	}
	
	SubShader{

	CGPROGRAM
	
	// compiler directive; instructs shader compiler to generate a surface shader with standard lighting and full support for shadows
	#pragma surface ConfigureSurface Standard fullforwardshadows addshadow

	// indicate surface shader needs to invoke a ConfigureProcedural function per vertex
	#pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural

	// turn off asynchronous shading so dummy shader isn't made when doing procedural drawing
	#pragma editor_sync_compilation


	// minimum for shader's target level and quality
	#pragma target 4.5

	// define input structure for configuration function
	struct Input {
		float3 worldPos;
	};

	struct InstanceData {
		float3 startPosition;
		float3 position; // position of star on mesh
	};

	float _Smoothness;
	float _Transparency;

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
			unity_ObjectToWorld._m00_m11_m22 = 0.2; // this is star scale
		#endif
	}

	// define ConfigureSurface method - result indicated by inout
	void ConfigureSurface (Input input, inout SurfaceOutputStandard surface) 
	{
		surface.Albedo = (1.0, 1.0, 1.0, _Transparency);
		surface.Smoothness = _Smoothness;
	}

	ENDCG
	
	}

	FallBack "Diffuse"
}
