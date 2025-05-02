Shader "Custom/PixelationURP"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _PixelSize("Pixel Size", Range(1, 256)) = 64
        _ColorCount("Color Count", Range(2, 32)) = 8 
        _DitherStrength("Dither Strength", Range(0, 1)) = 0.5 
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float _PixelSize;
            float _ColorCount;
            float _DitherStrength;
            
            float3 QuantizeColor(float3 color, float steps)
            {
                return floor(color * steps) / steps;
            }
            
            float DitherPattern(float2 uv)
            {
                float2 pixelPos = floor(uv * _PixelSize);
                return ((pixelPos.x + pixelPos.y) % 2) * _DitherStrength;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 pixelatedUV = floor(IN.uv * _PixelSize) / _PixelSize;
                
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, pixelatedUV);
                
                color.rgb = QuantizeColor(color.rgb, _ColorCount);
                
                color.rgb += DitherPattern(IN.uv) / _ColorCount;
                
                return color;
            }
            ENDHLSL
        }
    }
}