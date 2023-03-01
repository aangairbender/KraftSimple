Shader "Outlines/BackFaceOutlines"
{
    Properties
    {
        _Thickness ("Thickness", Float) = 1
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "Outlines"
            Cull Front

            HLSLPROGRAM
            // Standard URP requirements
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            // Register our functions
            #pragma vertex Vertex
            #pragma fragment Fragment

            // Include our logic file
            #include "BackFaceOutlines.hlsl"

            ENDHLSL
        }
    }
}
