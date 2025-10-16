Shader "Unlit/Noise"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        
        // 噪点参数
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.3  // 降低默认强度
        _NoiseScale ("Noise Scale", Range(0.1, 20)) = 5  // 降低默认值，使噪点更大更稀疏
        _NoiseDensity ("Noise Density", Range(0.1, 2)) = 0.7  // 新增：控制噪点稀疏度，值越小越稀疏
        _NoiseContrast ("Noise Contrast", Range(0.1, 5)) = 1
        _NoiseSpeed ("Noise Animation Speed", Range(0, 3)) = 0.5  // 降低默认动画速度
        
        // 遮罩参数
        _MaskThreshold ("Mask Threshold", Range(0, 1)) = 0.6  // 提高阈值，减少显示的噪点
        _MaskEdgeSmooth ("Mask Edge Smoothness", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // 声明变量
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float _NoiseIntensity;
            float _NoiseScale;
            float _NoiseDensity;  // 新增变量
            float _NoiseContrast;
            float _NoiseSpeed;
            float _MaskThreshold;
            float _MaskEdgeSmooth;

            // 改进的伪随机函数
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            // 改进的噪点函数 - 更稀疏自然
            float noise(float2 uv)
            {
                // 创建多个不同频率的随机值并叠加
                float value = 0.0;
                value += 0.5 * rand(uv);
                value += 0.25 * rand(uv * 2.0);
                value += 0.125 * rand(uv * 4.0);
                
                // 应用密度控制 - 值越小噪点越稀疏
                value = pow(value, _NoiseDensity);
                
                // 归一化并调整对比度
                value = (value - 0.5) * _NoiseContrast + 0.5;
                return clamp(value, 0.0, 1.0);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算动画偏移
                float time = _Time.y * _NoiseSpeed;
                float2 animatedUV = i.uv * _NoiseScale + float2(time * 0.1, time * 0.2);
                
                // 生成噪点
                float noiseValue = noise(animatedUV);
                
                // 计算遮罩 - 使用平滑阈值创建柔和边缘
                float mask = smoothstep(
                    _MaskThreshold - _MaskEdgeSmooth, 
                    _MaskThreshold + _MaskEdgeSmooth, 
                    noiseValue
                );
                
                // 获取主纹理颜色
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // 应用噪点遮罩效果
                col.rgb = lerp(col.rgb, noiseValue, _NoiseIntensity * mask);
                col.a = lerp(col.a, mask, _NoiseIntensity);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
