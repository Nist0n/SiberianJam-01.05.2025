Shader "Custom/6 Sided Pixelated"
{
//    Properties
//    {
//        _Tint ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
//        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
//        _Rotation ("Rotation", Range(0, 360)) = 0
//        [NoScaleOffset] _FrontTex ("Front [+Z] (HDR)", 2D) = "grey" {}
//        [NoScaleOffset] _BackTex ("Back [-Z] (HDR)", 2D) = "grey" {}
//        [NoScaleOffset] _LeftTex ("Left [+X] (HDR)", 2D) = "grey" {}
//        [NoScaleOffset] _RightTex ("Right [-X] (HDR)", 2D) = "grey" {}
//        [NoScaleOffset] _UpTex ("Up [+Y] (HDR)", 2D) = "grey" {}
//        [NoScaleOffset] _DownTex ("Down [-Y] (HDR)", 2D) = "grey" {}
//        
//        // Пикселизация
//        _PixelSize("Pixel Size", Range(1, 256)) = 64
//        _ColorSteps("Color Steps", Range(2, 64)) = 8
//        _DitherAmount("Dither Amount", Range(0, 0.1)) = 0.02
//    }
//
//    SubShader
//    {
//        Tags 
//        { 
//            "Queue" = "Background" 
//            "RenderType" = "Background" 
//            "PreviewType" = "Skybox" 
//            "RenderPipeline" = "UniversalPipeline"
//        }
//
//        Pass
//        {
//            ZWrite Off
//            Cull Off
//            
//            HLSLPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma multi_compile _ _PIXELATION_ON
//
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//
//            struct Attributes
//            {
//                float4 positionOS : POSITION;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct Varyings
//            {
//                float4 positionHCS : SV_POSITION;
//                float2 uv : TEXCOORD0;
//                float3 texCoord : TEXCOORD1;
//            };
//
//            // Текстуры и параметры
//            TEXTURE2D(_FrontTex); SAMPLER(sampler_FrontTex);
//            TEXTURE2D(_BackTex);  SAMPLER(sampler_BackTex);
//            TEXTURE2D(_LeftTex);  SAMPLER(sampler_LeftTex);
//            TEXTURE2D(_RightTex); SAMPLER(sampler_RightTex);
//            TEXTURE2D(_UpTex);    SAMPLER(sampler_UpTex);
//            TEXTURE2D(_DownTex);  SAMPLER(sampler_DownTex);
//
//            CBUFFER_START(UnityPerMaterial)
//                half4 _Tint;
//                half _Exposure;
//                float _Rotation;
//                float4 _FrontTex_HDR;
//                float4 _BackTex_HDR;
//                float4 _LeftTex_HDR;
//                float4 _RightTex_HDR;
//                float4 _UpTex_HDR;
//                float4 _DownTex_HDR;
//                
//                // Пикселизация
//                float _PixelSize;
//                float _ColorSteps;
//                float _DitherAmount;
//            CBUFFER_END
//
//            // Функция квантования цвета
//            half3 QuantizeColor(half3 color, float steps)
//            {
//                return floor(color * steps) / steps;
//            }
//
//            // Простой дизеринг
//            float Dither(float2 uv)
//            {
//                float2 pixelPos = floor(uv * _PixelSize);
//                return ((pixelPos.x + pixelPos.y) % 2) * _DitherAmount;
//            }
//
//            // Функция пикселизации
//            float2 PixelateUV(float2 uv)
//            {
//                float2 pixelScale = _ScreenParams.xy / _PixelSize;
//                float2 pixelatedUV = floor(uv * pixelScale) / pixelScale;
//                return pixelatedUV;
//            }
//
//            Varyings vert(Attributes IN)
//            {
//                Varyings OUT;
//                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
//                
//                // Вращение UV
//                float rad = radians(_Rotation);
//                float cosRot = cos(rad);
//                float sinRot = sin(rad);
//                float2x2 rotationMatrix = float2x2(cosRot, -sinRot, sinRot, cosRot);
//                OUT.uv = mul(IN.uv - 0.5, rotationMatrix) + 0.5;
//                OUT.texCoord = IN.positionOS.xyz;
//                return OUT;
//            }
//
//            half4 frag(Varyings IN) : SV_Target
//            {
//                // Определяем направление для выбора текстуры
//                float3 coord = normalize(IN.texCoord);
//                float2 uv;
//                half4 texColor;
//                half4 hdrColor;
//                
//                if (abs(coord.z) > abs(coord.x) && abs(coord.z) > abs(coord.y))
//                {
//                    uv = coord.z > 0 ? IN.uv : float2(1.0 - IN.uv.x, IN.uv.y);
//                    uv = PixelateUV(uv); // Пикселизация
//                    texColor = coord.z > 0 ? SAMPLE_TEXTURE2D(_FrontTex, sampler_FrontTex, uv) 
//                                          : SAMPLE_TEXTURE2D(_BackTex, sampler_BackTex, uv);
//                    hdrColor = coord.z > 0 ? _FrontTex_HDR : _BackTex_HDR;
//                }
//                else if (abs(coord.x) > abs(coord.y))
//                {
//                    uv = coord.x > 0 ? float2(1.0 - IN.uv.y, IN.uv.x) : IN.uv.yx;
//                    uv = PixelateUV(uv);
//                    texColor = coord.x > 0 ? SAMPLE_TEXTURE2D(_RightTex, sampler_RightTex, uv) 
//                                          : SAMPLE_TEXTURE2D(_LeftTex, sampler_LeftTex, uv);
//                    hdrColor = coord.x > 0 ? _RightTex_HDR : _LeftTex_HDR;
//                }
//                else
//                {
//                    uv = coord.y > 0 ? float2(IN.uv.x, 1.0 - IN.uv.y) : IN.uv;
//                    uv = PixelateUV(uv);
//                    texColor = coord.y > 0 ? SAMPLE_TEXTURE2D(_UpTex, sampler_UpTex, uv) 
//                                          : SAMPLE_TEXTURE2D(_DownTex, sampler_DownTex, uv);
//                    hdrColor = coord.y > 0 ? _UpTex_HDR : _DownTex_HDR;
//                }
//
//                // Декодинг HDR и применение экспозиции
//                half3 color = DecodeHDR(texColor, hdrColor) * _Tint.rgb * _Exposure;
//                
//                // Квантование цвета и дизеринг
//                color = QuantizeColor(color, _ColorSteps);
//                color += Dither(uv) / _ColorSteps;
//                
//                return half4(color, 1.0);
//            }
//            ENDHLSL
//        }
//    }
//    Fallback "Skybox/6 Sided"
}