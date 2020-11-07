Shader "Anker/SimpleBlur"
{
    Properties
    {
        _Strength("Strength", Range(0, 10)) = 1
    }

    Category
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Opaque"
        }

        SubShader
        {
            GrabPass
            {
                Tags { "LightMode" = "Always" }
            }

            Pass
            {
                Tags { "LightMode" = "Always" }

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : POSITION;
                    float4 color : COLOR;
                    float4 uv : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                    #else
                        float scale = 1.0;
                    #endif
                    o.uv.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                    o.uv.zw = o.vertex.zw;
                    return o;
                }

                static const int KernelSize = 16;
                static const float2 Kernel[KernelSize] = {
                    float2(0, 0),
                    float2(0.54545456, 0),
                    float2(0.16855472, 0.5187581),
                    float2(-0.44128203, 0.3206101),
                    float2(-0.44128197, -0.3206102),
                    float2(0.1685548, -0.5187581),
                    float2(1, 0),
                    float2(0.809017, 0.58778524),
                    float2(0.30901697, 0.95105654),
                    float2(-0.30901703, 0.9510565),
                    float2(-0.80901706, 0.5877852),
                    float2(-1, 0),
                    float2(-0.80901694, -0.58778536),
                    float2(-0.30901664, -0.9510566),
                    float2(0.30901712, -0.9510565),
                    float2(0.80901694, -0.5877853),
                };

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;

                float _Strength;

                half4 frag(v2f i) : COLOR
                {
                    half4 color = 0;
                    for (int k = 0; k < KernelSize; k++)
                    {
                        half2 offset = Kernel[k] * _GrabTexture_TexelSize.xy * _Strength;
                        color += tex2D(_GrabTexture, i.uv + offset);
                    }
                    color *= i.color / KernelSize;
                    return color;
                }

                ENDCG
            }
        }
    }
}
