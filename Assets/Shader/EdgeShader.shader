Shader "Unlit/EdgeShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}  // 基础纹理（可选）
        _CenterColor ("Center Color", Color) = (1,1,1,1)         // 中心颜色
        _EdgeColor ("Edge Color", Color) = (0,0,0,1)             // 边缘颜色
        _FadeRange ("Fade Range", Range(0, 1)) = 0.8             // 渐变范围（0=全边缘色，1=中心色范围大）
        _Softness ("Fade Softness", Range(0.01, 0.5)) = 0.2      // 过渡柔和度
        _AlphaStrength ("Alpha Strength", Range(0, 1)) = 1.0     // 整体透明度强度（1=完全不透明中心）
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : TEXCOORD1;
                float2 localPos : TEXCOORD2;  // 局部UV坐标（0~1）
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _CenterColor;
            float4 _EdgeColor;
            float _FadeRange;
            float _Softness;
            float _AlphaStrength;
            float4 _MainTex_TexelSize;  // 纹理尺寸，用于比例校正

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;  // 接收UI组件的颜色 tint
                o.localPos = v.uv;  // UV坐标（0~1范围）
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样基础纹理（如果不需要纹理，可注释此行，直接用颜色）
                fixed4 texColor = tex2D(_MainTex, i.uv) * i.color;

                // 计算中心坐标（UV中心为(0.5,0.5)）
                float2 center = float2(0.5, 0.5);
                
                // 校正纹理宽高比，防止方形变形
                float aspectRatio = _MainTex_TexelSize.z / _MainTex_TexelSize.w;  // 宽/高
                float2 normalizedPos = (i.localPos - center) * float2(aspectRatio, 1.0);
                
                // 计算方形距离（核心：用max获取x/y方向上的最大偏移，形成方形轮廓）
                float squareDistance = max(abs(normalizedPos.x), abs(normalizedPos.y));
                
                // 归一化距离（0=中心，1=方形边缘）
                float normalizedDistance = squareDistance / 0.5;  // 0.5是中心到边缘的最大距离

                // 计算颜色混合因子（0=中心色，1=边缘色）
                float colorLerp = smoothstep(_FadeRange - _Softness, _FadeRange, normalizedDistance);
                
                // 混合中心色和边缘色，并叠加纹理颜色
                float3 finalColor = lerp(_CenterColor.rgb, _EdgeColor.rgb, colorLerp) * texColor.rgb;
                
                // 计算透明度（中心完全不透明，边缘透明）
                float alpha = (1 - colorLerp) * _AlphaStrength * _CenterColor.a * texColor.a;

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
