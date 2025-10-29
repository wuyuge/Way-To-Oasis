Shader "Custom/Circle"
{
    Properties
    {
        _MainColor ("Fill Color", Color) = (1,1,1,1) // 填充色（中空部分会忽略此颜色）
        _BorderColor ("Border Color", Color) = (1,1,1,1) // 边框颜色
        _CircleSize ("Circle Size", Range(0, 1)) = 0.5 // 圆形整体大小（0-1范围，基于UV）
        _BorderWidth ("Border Width", Range(0, 0.5)) = 0.1 // 边框宽度
        _Softness ("Border Softness", Range(0, 0.1)) = 0.01 // 边框边缘柔和度
        _AspectRatio ("Aspect Ratio", Float) = 1 // 宽高比（用于校正非正方形UV的变形）
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off // 双面可见
        ZWrite Off // 不写入深度缓存（避免遮挡透明物体）

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _MainColor;
            float4 _BorderColor;
            float _CircleSize;
            float _BorderWidth;
            float _Softness;
            float _AspectRatio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // 使用原始UV坐标
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 将UV坐标转换为以中心为原点（-0.5到0.5范围）
                float2 uv = i.uv - 0.5;
                
                // 校正宽高比，避免圆形变成椭圆
                uv.x *= _AspectRatio;
                
                // 计算当前像素到中心的距离
                float distance = length(uv);
                
                // 计算圆形外半径（整体大小的一半）
                float outerRadius = _CircleSize * 0.5;
                
                // 计算圆形内半径（外半径 - 边框宽度）
                float innerRadius = outerRadius - _BorderWidth;
                
                // 计算边框的内外边缘（加入柔和度）
                float outerEdge = outerRadius + _Softness * 0.5;
                float innerEdge = innerRadius - _Softness * 0.5;
                
                // 计算边框的不透明度（外边缘到内边缘之间渐变）
                float outerMask = 1 - smoothstep(outerRadius - _Softness * 0.5, outerEdge, distance);
                float innerMask = smoothstep(innerEdge, innerRadius + _Softness * 0.5, distance);
                float borderMask = outerMask * innerMask;
                
                // 最终颜色 = 边框颜色 * 边框透明度
                fixed4 col = _BorderColor * borderMask;
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}