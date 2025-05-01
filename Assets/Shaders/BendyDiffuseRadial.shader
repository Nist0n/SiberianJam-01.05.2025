Shader "Custom/BendyTransparentRadial_URP"
{
    Properties
    {
        _BaseColor ("Main Color", Color) = (1, 1, 1, 0.5)  // Для прозрачности
        _BaseMap ("Base (RGB) Alpha (A)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5 
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"  
            "IgnoreProjector" = "True"  
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha  
            ZWrite Off  
            Cull Back  

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ HORIZON_WAVES
            #pragma multi_compile __ BEND_ON
            #pragma shader_feature _ALPHATEST_ON  

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            half3 _CurveOrigin;
            half3 _ReferenceDirection;
            half _Curvature;
            half3 _Scale;
            half _FlatMargin;
            half _HorizonWaveFrequency;
            
            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            half4 _BaseColor;
            half _Cutoff;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            half4 Bend(half4 positionOS)
            {
                half4 positionWS = mul(UNITY_MATRIX_M, positionOS);
                
                half2 xzDist = (positionWS.xz - _CurveOrigin.xz) / _Scale.xz;
                half dist = length(xzDist);
                half waveMultiplier = 1.0;
                
                #if defined(HORIZON_WAVES)
                half2 direction = lerp(_ReferenceDirection.xz, xzDist, min(dist, 1.0));
                half theta = acos(clamp(dot(normalize(direction), normalize(_ReferenceDirection.xz)), -1.0, 1.0));
                waveMultiplier = cos(theta * _HorizonWaveFrequency);
                #endif
                
                dist = max(0.0, dist - _FlatMargin);
                positionWS.y -= dist * dist * _Curvature * waveMultiplier;
                
                return mul(UNITY_MATRIX_I_M, positionWS);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                #if defined(BEND_ON)
                IN.positionOS = Bend(IN.positionOS);
                #endif
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = tex2D(_BaseMap, IN.uv) * _BaseColor;
                
                #ifdef _ALPHATEST_ON
                clip(color.a - _Cutoff);
                #endif
                
                return color;
            }
            ENDHLSL
        }
    }

    Fallback "Universal Render Pipeline/Transparent"
}