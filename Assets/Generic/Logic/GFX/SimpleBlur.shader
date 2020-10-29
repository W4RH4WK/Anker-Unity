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
                    float4 uvgrab : TEXCOORD0;
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
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }

                #define KernelSize 7
                static const float Kernel[KernelSize][KernelSize] = {
                    {0.000036f, 0.000363f, 0.001446f, 0.002291f, 0.001446f, 0.000363f, 0.000036f},
                    {0.000363f, 0.003676f, 0.014662f, 0.023226f, 0.014662f, 0.003676f, 0.000363f},
                    {0.001446f, 0.014662f, 0.058488f, 0.092651f, 0.058488f, 0.014662f, 0.001446f},
                    {0.002291f, 0.023226f, 0.092651f, 0.146768f, 0.092651f, 0.023226f, 0.002291f},
                    {0.001446f, 0.014662f, 0.058488f, 0.092651f, 0.058488f, 0.014662f, 0.001446f},
                    {0.000363f, 0.003676f, 0.014662f, 0.023226f, 0.014662f, 0.003676f, 0.000363f},
                    {0.000036f, 0.000363f, 0.001446f, 0.002291f, 0.001446f, 0.000363f, 0.000036f},
                };

                #define GRABXYPIXEL(kernelx, kernely) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely, i.uvgrab.z, i.uvgrab.w)))

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;

                float _Strength;

                half4 frag(v2f i) : COLOR
                {
                    half4 sum = half4(0,0,0,0);
                    
                    for (int y = -KernelSize/2; y < KernelSize/2; y++)
                    {
                        for (int x = -KernelSize/2; x < KernelSize/2; x++)
                        {
                            sum += Kernel[x + KernelSize/2][y + KernelSize/2] * GRABXYPIXEL(_Strength * x, _Strength * y);
                        }
                    }

                    return sum * i.color;
                }

                ENDCG
            }
        }
    }
}
