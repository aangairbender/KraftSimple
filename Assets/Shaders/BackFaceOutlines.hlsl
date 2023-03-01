#ifndef BACKFACEOUTLINES_INCLUDED
#define BACKFACEOUTLINES_INCLUDED

// Include helper functions from URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

// Data from the meshes
struct Attributes {
	float4 positionOS : POSITION; // Position in object space
	float3 normalOS : NORMAL; // Normal in object space
};


struct VertexOutput {
	float4 positionCS : SV_POSITION; // Position in clip space
};

// Properties
float _Thickness;
float4 _Color;

VertexOutput Vertex(Attributes input) {
	VertexOutput output;

	float3 posOS = input.positionOS.xyz + input.normalOS * _Thickness;
	output.positionCS = GetVertexPositionInputs(posOS).positionCS;

	return output;
}

float4 Fragment(VertexOutput input) : SV_TARGET {
	return _Color;
};

#endif