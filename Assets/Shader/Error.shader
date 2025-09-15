Shader "Custom/ScreenGlitch"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}  // 屏幕原图
        _GlitchIntensity ("Glitch Strength", Range(0, 0.2)) = 0.02  // 整体干扰强度
        _NoiseIntensity ("Noise Strength", Range(0, 1)) = 0.3  // 雪花噪点强度
        _ColorShift ("Color Shift", Range(0, 0.05)) = 0.01  // 颜色偏移强度
        _GlitchSpeed ("Glitch Speed", Range(0, 10)) = 5  // 干扰动画速度
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // 输入参数
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _GlitchIntensity;
            float _NoiseIntensity;
            float _ColorShift;
            float _GlitchSpeed;

            // 顶点输入结构
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // 顶点输出结构
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // 顶点着色器（仅传递UV和位置）
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 简单随机函数（生成噪点用）
            float Random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 片段着色器（核心干扰逻辑）
            fixed4 frag (v2f i) : SV_Target
            {
                // 1. 时间参数（控制干扰动画）
                float time = _Time.y * _GlitchSpeed;

                // 2. 生成UV抖动（模拟信号不稳定）
                float2 glitchUV = i.uv;
                // 水平抖动：随时间和UV随机偏移
                float horizontalGlitch = (Random(float2(time, i.uv.y)) - 0.5) * _GlitchIntensity;
                // 垂直抖动：按行随机偏移（更贴近真实屏幕干扰）
                float verticalGlitch = (Random(float2(i.uv.x, floor(time * 5))) - 0.5) * _GlitchIntensity;
                glitchUV += float2(horizontalGlitch, verticalGlitch);

                // 3. 颜色通道分离（模拟信号串色）
                fixed4 col;
                col.r = tex2D(_MainTex, glitchUV + float2(_ColorShift, 0)).r;  // 红色通道右移
                col.g = tex2D(_MainTex, glitchUV).g;                          // 绿色通道不变
                col.b = tex2D(_MainTex, glitchUV - float2(_ColorShift, 0)).b;  // 蓝色通道左移
                col.a = 1;

                // 4. 叠加雪花噪点（模拟信号杂波）
                float noise = Random(float2(i.uv.x * 1000, i.uv.y * 1000 + time * 10)) * _NoiseIntensity;
                col.rgb = lerp(col.rgb, float3(noise, noise, noise), noise);  // 噪点与原图混合

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"  // 降级方案（兼容旧管线）
}