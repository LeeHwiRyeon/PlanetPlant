Shader "Custom/PlanetSprite"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _ClipTex1("Clipping Texture1", 2D) = "white" {}
        _ClipTex2("Clipping Texture2", 2D) = "white" {}
        _UpperClipThreshold("Upper Clipping Threshold", Range(0, 1)) = 0
        _LowerClipThreshold("Lower Clipping Threshold", Range(0, 1)) = 0
        _BorderColor("Border Color", Color) = (1, 1, 1, 1)
        _BorderWidth("Border Width", Range(0, 0.5)) = 0.05
        _AtlasSize("Atlas Size", Vector) = (1, 1, 0, 0)
        _Segments("Segments", Range(1, 64)) = 4
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" "CanUseSpriteAtlas"="True" "PreviewType"="Plane" "RenderPipeline"="UniversalPipeline"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass {
            HLSLPROGRAM
            #pragma vertex vertexFunction
            #pragma fragment fragmentFunction
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"
            #define PI 3.14159265

            struct InputAttributes
            {
                float3 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _ClipTex1;
            sampler2D _ClipTex2;
            half _UpperClipThreshold;
            half _LowerClipThreshold;
            float4 _BorderColor;
            half _BorderWidth;
            float2 _AtlasSize;
            int _Segments;
            float _MantlePieces[64];

            Interpolators vertexFunction(InputAttributes input)
            {
                Interpolators output;
                output.vertex = TransformObjectToHClip(input.vertex);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            float2 calculateAtlasUV(float2 uv)
            {
                int segments = round(_Segments);
                float segmentAngle = 2.0 * PI / segments;
                float angle = atan2(uv.y - 0.5, uv.x - 0.5);

                angle = (angle + 2.0 * PI) % (2.0 * PI); // 수정된 부분

                int tileIndex = floor(angle / segmentAngle);
                tileIndex = floor(_MantlePieces[tileIndex]);

                float2 segmentSize = 1.0 / _AtlasSize;
                float2 segmentOffset = float2(tileIndex % (int)_AtlasSize.x, floor(tileIndex / _AtlasSize.x)) * segmentSize;
                return uv * segmentSize + segmentOffset;
            }

            half4 applyTransparencyAndBorders(half4 color, half radius, half edgeDist)
            {
                half borderMix = smoothstep(_BorderWidth
                    * 0.5 - edgeDist, _BorderWidth * 1.5 + edgeDist, radius);

                // 테두리 색상과 원래 색상을 혼합하여 최종 색상을 얻습니다.
                color.rgb = lerp(color.rgb, _BorderColor.rgb, _BorderColor.a * (1 - borderMix));

                // 혼합 비율에 따라 투명도를 조절합니다.
                color.a *= borderMix;

                return color;
            }

            half4 fragmentFunction(Interpolators interpolators) : SV_Target
            {
                half4 finalColor;

                half x = (-0.5 + interpolators.uv.x) * 2;
                half y = (-0.5 + interpolators.uv.y) * 2;
                half radius = 1 - sqrt(x * x + y * y);
                half edgeDist = fwidth(radius);

                if (radius > _UpperClipThreshold) {
                    finalColor = tex2D(_ClipTex1, interpolators.uv) * interpolators.color;
                    half innerRadius = smoothstep(_UpperClipThreshold - edgeDist, _UpperClipThreshold + edgeDist, radius);
                    finalColor.a *= innerRadius;
                } else {
                    float2 atlasUV = calculateAtlasUV(interpolators.uv);
                    finalColor = tex2D(_ClipTex2, atlasUV) * interpolators.color;
                    half aaRadius = smoothstep(_LowerClipThreshold - edgeDist, _LowerClipThreshold + edgeDist, radius);
                    finalColor.a *= aaRadius;
                }

                finalColor = applyTransparencyAndBorders(finalColor, radius, edgeDist);

                return finalColor;
            }

            ENDHLSL
        }
    }
}
