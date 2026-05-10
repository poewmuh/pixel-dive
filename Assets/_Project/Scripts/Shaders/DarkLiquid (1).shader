Shader "Custom/DarkLiquidImproved"
{
    Properties
    {
        // --- Liquid visuals ---
        _ColorA        ("Dark Color A",         Color)  = (0.01, 0.01, 0.06, 1)
        _ColorB        ("Accent Color B",       Color)  = (0.04, 0.04, 0.14, 1)
        _ColorC        ("Highlight Color C",    Color)  = (0.08, 0.08, 0.22, 1)
        _Speed         ("Flow Speed",           Float)  = 0.25
        _Scale         ("Noise Scale",          Float)  = 2.0
        _WarpStrength  ("Warp Strength",        Float)  = 1.8
        _Contrast      ("Contrast",             Float)  = 1.4

        // --- Player reveal ---
        _PlayerWorldPos("Player World Pos",     Vector) = (0, 0, 0, 0)
        _RevealRadius  ("Reveal Radius (units)", Float) = 2.5
        _RevealEdge    ("Reveal Edge Softness", Float)  = 1.2

        // --- Required by SpriteRenderer ---
        [HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint",             Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"  = "Transparent"
            "Queue"       = "Transparent+100"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "DarkLiquidPass"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ─────────────────────────────────────────────
            //  Uniforms
            // ─────────────────────────────────────────────
            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float4 _ColorC;
                float  _Speed;
                float  _Scale;
                float  _WarpStrength;
                float  _Contrast;

                float4 _PlayerWorldPos;
                float  _RevealRadius;
                float  _RevealEdge;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // ─────────────────────────────────────────────
            //  Structs
            // ─────────────────────────────────────────────
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float4 color       : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ─────────────────────────────────────────────
            //  Math helpers
            // ─────────────────────────────────────────────

            // Hash — fast, no texture lookup needed
            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            // Gradient noise  (range −1 … +1)
            float gnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);   // smoothstep

                float a = dot(hash2(i + float2(0,0)), f - float2(0,0));
                float b = dot(hash2(i + float2(1,0)), f - float2(1,0));
                float c = dot(hash2(i + float2(0,1)), f - float2(0,1));
                float d = dot(hash2(i + float2(1,1)), f - float2(1,1));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // Fractal Brownian Motion — 4 octaves
            float fbm(float2 p)
            {
                float v = 0.0;
                float amp = 0.5;
                float2x2 rot = float2x2(1.6,  1.2, -1.2, 1.6); // rotate & scale each octave

                for (int i = 0; i < 4; i++)
                {
                    v   += amp * gnoise(p);
                    p    = mul(rot, p);
                    amp *= 0.5;
                }
                return v;
            }

            // Матрица медленного вращения всего UV-пространства
            float2 slowRotate(float2 p, float t)
            {
                float angle = t * 0.04; // очень медленный глобальный поворот
                float s = sin(angle);
                float c = cos(angle);
                return float2(c * p.x - s * p.y, s * p.x + c * p.y);
            }

            // Domain Warping с круговыми смещениями вместо линейного дрейфа
            float liquidNoise(float2 p, float t)
            {
                // Медленно вращаем всё пространство — базовые завихрения
                p = slowRotate(p, t);

                // Первый слой: смещения по окружности (sin/cos) → нет направленного дрейфа
                float2 q = float2(
                    fbm(p + float2(cos(t * 0.31), sin(t * 0.23))),
                    fbm(p + float2(sin(t * 0.27), cos(t * 0.19)) + float2(5.2, 1.3))
                );

                // Второй слой: другие частоты → завихрения внутри завихрений
                float2 r = float2(
                    fbm(p + _WarpStrength * q + float2(cos(t * 0.13 + 1.7), sin(t * 0.17 + 9.2))),
                    fbm(p + _WarpStrength * q + float2(sin(t * 0.11 + 8.3), cos(t * 0.09 + 2.8)))
                );

                return fbm(p + _WarpStrength * r);
            }

            // ─────────────────────────────────────────────
            //  Vertex
            // ─────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.color       = IN.color;
                return OUT;
            }

            // ─────────────────────────────────────────────
            //  Fragment
            // ─────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _Speed;
                float2 p = IN.uv * _Scale;

                // ── Liquid noise value in [~-1..1] ──
                float n = liquidNoise(p, t);

                // Remap to [0..1] and boost contrast
                n = saturate(n * 0.5 + 0.5);
                n = pow(n, _Contrast);

                // ── Three-way colour blend ──
                float3 col;
                if (n < 0.45)
                    col = lerp(_ColorA.rgb, _ColorB.rgb, n / 0.45);
                else
                    col = lerp(_ColorB.rgb, _ColorC.rgb, (n - 0.45) / 0.55);

                // ── Player reveal (world-space circle) ──
                float dist = distance(IN.worldPos.xy, _PlayerWorldPos.xy);

                // alpha: 1 = full darkness,  0 = transparent (player sees through)
                float revealAlpha = smoothstep(
                    _RevealRadius - _RevealEdge,
                    _RevealRadius,
                    dist
                );

                return half4(col, revealAlpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
