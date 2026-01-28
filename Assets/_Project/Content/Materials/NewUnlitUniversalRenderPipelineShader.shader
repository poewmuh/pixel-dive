Shader "Photoshop/Overlay_Extreme_Performance"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }

        Cull Off Lighting Off ZWrite Off Fog { Mode Off }
        Blend DstColor SrcColor

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert(appdata_full v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Прямое чтение текстуры
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // Вычисляем альфу один раз
                fixed alpha = tex.a * i.color.a;

                // Оптимизированный lerp: 
                // Вместо ler(0.5, tex*color*2, alpha) 
                // делаем математически то же самое, но чуть "ближе к металлу"
                fixed3 overlay = tex.rgb * i.color.rgb * 1.5;
                fixed3 res = 0.5 + (overlay - 0.5) * alpha;

                return fixed4(res, 1.0);
            }
            ENDCG
        }
    }
}