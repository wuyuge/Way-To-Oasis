Shader "Custom/ImageBreak" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BreakAmount ("BreakAmount", Range(0, 1)) = 0.5
        _NoiseScale ("NoiseScale", Float) = 5.0
        _EdgeWidth ("EdgeWidth", Range(0, 0.2)) = 0.05
        _EdgeThickness ("EdgeThickness", Range(1, 10)) = 3.0
        _EdgeColor ("EdgeColor", Color) = (0,0,0,1)
        _EdgeIntensity ("EdgeIntensity", Range(1, 5)) = 2.0
        // 移除自定义Alpha参数
    }

    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
            "UI"="True" // 标记为UI Shader，适配UI渲染
        }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc" // 引入UI相关宏，适配UI顶点色

            // 噪声函数
            float rand(float2 co) {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // 接收UI传递的顶点色（包含Image的Alpha）
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; // 传递顶点色到片元着色器
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BreakAmount;
            float _NoiseScale;
            float _EdgeWidth;
            float _EdgeThickness;
            float4 _EdgeColor;
            float _EdgeIntensity;
            // 移除自定义Alpha变量

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color; // 传递Image的顶点色（包含Alpha）
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 生成噪声图案
                float n = noise(i.uv * _NoiseScale);
                
                // 破碎掩码
                float breakMask = step(_BreakAmount, n);
                
                // 边缘检测：基于厚度调整采样距离
                float thicknessFactor = _EdgeThickness * 0.001;
                float2 offset = (thicknessFactor / _NoiseScale) * 2;
                
                // 多方向采样
                float n1 = noise(float2(i.uv.x + offset.x, i.uv.y) * _NoiseScale);
                float n2 = noise(float2(i.uv.x - offset.x, i.uv.y) * _NoiseScale);
                float n3 = noise(float2(i.uv.x, i.uv.y + offset.y) * _NoiseScale);
                float n4 = noise(float2(i.uv.x, i.uv.y - offset.y) * _NoiseScale);
                float n5 = noise(float2(i.uv.x + offset.x*0.7, i.uv.y + offset.y*0.7) * _NoiseScale);
                float n6 = noise(float2(i.uv.x - offset.x*0.7, i.uv.y - offset.y*0.7) * _NoiseScale);
                float n7 = noise(float2(i.uv.x + offset.x*0.7, i.uv.y - offset.y*0.7) * _NoiseScale);
                float n8 = noise(float2(i.uv.x - offset.x*0.7, i.uv.y + offset.y*0.7) * _NoiseScale);
                float n9 = noise(float2(i.uv.x + offset.x*1.3, i.uv.y) * _NoiseScale);
                float n10 = noise(float2(i.uv.x - offset.x*1.3, i.uv.y) * _NoiseScale);
                float n11 = noise(float2(i.uv.x, i.uv.y + offset.x*1.3) * _NoiseScale);
                float n12 = noise(float2(i.uv.x, i.uv.y - offset.x*1.3) * _NoiseScale);
                
                // 计算边缘因子
                float edgeSum = step(_BreakAmount, n1) + step(_BreakAmount, n2) + 
                               step(_BreakAmount, n3) + step(_BreakAmount, n4) +
                               step(_BreakAmount, n5) + step(_BreakAmount, n6) +
                               step(_BreakAmount, n7) + step(_BreakAmount, n8) +
                               step(_BreakAmount, n9) + step(_BreakAmount, n10) +
                               step(_BreakAmount, n11) + step(_BreakAmount, n12);
                float edge = 1 - (edgeSum / 12.0);
                
                // 应用厚度和宽度控制
                edge = smoothstep(0, _EdgeWidth * (2 + _EdgeThickness * 0.1), edge);
                edge = pow(edge, 1.0 / _EdgeIntensity);
                
                // 获取原图颜色
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 1. 应用破碎效果的 alpha
                col.a *= breakMask;
                
                // 2. 计算边缘 alpha（限制在 0-1 范围内）
                float edgeAlpha = saturate(edge * _EdgeColor.a * _EdgeIntensity * (1 + _EdgeThickness * 0.1));
                
                // 3. 叠加边缘颜色（正确的 lerp 逻辑）
                col.rgb = lerp(col.rgb, _EdgeColor.rgb, edgeAlpha * (1 - col.a));
                
                // 4. 应用Image的Alpha（核心修改：使用顶点色的Alpha）
                col.a *= i.color.a;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Universal Render Pipeline/Unlit"
}