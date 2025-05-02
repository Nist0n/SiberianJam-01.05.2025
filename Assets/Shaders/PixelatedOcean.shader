Shader "Custom/PixelatedOcean"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        _PixelSize("Pixel Size", Range(1, 256)) = 64
        _ColorCount("Color Count", Range(2, 32)) = 8
        _DitherStrength("Dither Strength", Range(0, 1)) = 0.5
        _WaveSpeed("Wave Speed", Range(0, 5)) = 1.0
        _WaveHeight("Wave Height", Range(0, 0.5)) = 0.1
        _WaveFrequency("Wave Frequency", Range(0, 50)) = 10
        _OceanColor("Ocean Color", Color) = (0.1, 0.3, 0.5, 1)
        _FoamColor("Foam Color", Color) = (0.8, 0.9, 1.0, 1)
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
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float _PixelSize;
            float _ColorCount;
            float _DitherStrength;
            float _WaveSpeed;
            float _WaveHeight;
            float _WaveFrequency;
            half4 _OceanColor;
            half4 _FoamColor;
            
            float Wave(float2 pos, float time)
            {
                float wave = sin(pos.x * _WaveFrequency + time * _WaveSpeed);
                wave += sin(pos.x * _WaveFrequency * 2.3 + time * _WaveSpeed * 1.7) * 0.5;
                wave *= _WaveHeight;
                return wave;
            }
            
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
                
                float wave = Wave(IN.positionOS.xz, _Time.y);
                IN.positionOS.y += wave;
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float waveValue = Wave(IN.worldPos.xz, _Time.y);
                
                float foam = smoothstep(0.3, 0.5, waveValue);
                
                half3 oceanColor = lerp(_OceanColor.rgb, _FoamColor.rgb, foam);
                
                oceanColor = QuantizeColor(oceanColor, _ColorCount);
                
                oceanColor += DitherPattern(IN.uv) / _ColorCount;
                
                return half4(oceanColor, 1.0);
            }
            ENDHLSL
        }
    }
}