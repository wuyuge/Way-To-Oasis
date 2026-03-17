Shader "Custom/UIPageFlip_Center"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        
        [Header(Flip Settings)]
        _FlipAngle("翻转角度", Range(0, 180)) = 0
        _PageCurvature("页面弯曲度", Range(0, 0.5)) = 0.2
        _FlipSide("翻转方向", Range(-1, 1)) = 1 // -1=向左翻, 1=向右翻
        _ShadowStrength("阴影强度", Range(0, 1)) = 0.5
        
        [Header(Page Settings)]
        _LeftPageTexture("左页纹理", 2D) = "white" {}
        _RightPageTexture("右页纹理", 2D) = "white" {}
        _BackColor("背面颜色", Color) = (0.9,0.9,0.9,1)
        
        [Header(Book Binding)]
        _BindingWidth("书脊宽度", Range(0, 0.2)) = 0.05
        _BindingColor("书脊颜色", Color) = (0.5,0.3,0.2,1)
        
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float flipState : TEXCOORD2; // 0=左页正面, 1=右页正面, 2=背面
                float curvature : TEXCOORD3;
                float side : TEXCOORD4; // 0=左页, 1=右页
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _LeftPageTexture;
            sampler2D _RightPageTexture;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            
            float _FlipAngle;
            float _PageCurvature;
            float _FlipSide;
            float _ShadowStrength;
            fixed4 _BackColor;
            float _BindingWidth;
            fixed4 _BindingColor;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.worldPosition = v.vertex;
                
                float angle = radians(_FlipAngle);
                float flipSide = _FlipSide;
                
                float4 pos = v.vertex;
                float2 uv = v.texcoord;
                
                // 判断当前是在左页还是右页（基于UV.x）
                // 假设整个UI元素代表打开的书，中心在UV.x=0.5
                float isRightPage = step(0.5, uv.x);
                float isLeftPage = 1 - isRightPage;
                
                // 计算相对于书脊的位置（书脊在UV.x=0.5）
                float centerX = 0.5;
                float relativeX = uv.x - centerX; // -0.5 到 0.5
                float absRelativeX = abs(relativeX);
                
                // 书脊区域
                float isBinding = step(centerX - _BindingWidth, uv.x) * step(uv.x, centerX + _BindingWidth);
                
                // 翻转区域：根据翻转方向决定哪一页在翻转
                float flippingPage = 0;
                float flippingIntensity = 0;
                
                if (flipSide > 0) // 向右翻（右页翻转）
                {
                    flippingPage = isRightPage;
                    flippingIntensity = flippingPage * sin(angle);
                }
                else // 向左翻（左页翻转）
                {
                    flippingPage = isLeftPage;
                    flippingIntensity = flippingPage * sin(angle);
                }
                
                // 计算翻页效果
                if (flippingPage > 0.5 && angle > 0.01)
                {
                    // 计算相对于书脊的距离
                    float distanceFromSpine = absRelativeX;
                    
                    // 弯曲效果 - 基于距离书脊的距离
                    float bendFactor = sin(angle) * _PageCurvature;
                    float curvatureAmount = bendFactor * distanceFromSpine * 2; // 归一化到0-1
                    
                    // 添加Y方向的弯曲
                    float yBend = curvatureAmount * sin(uv.y * 3.14159);
                    
                    // 计算旋转角度 - 基于距离书脊的距离
                    float rotateAngle = angle * distanceFromSpine * 2 * flipSide;
                    
                    // 应用旋转变换
                    float cosAngle = cos(rotateAngle);
                    float sinAngle = sin(rotateAngle);
                    
                    // 旋转轴在书脊位置
                    float3 pivot = float3(centerX, 0, 0);
                    
                    // 相对于书脊的坐标
                    float3 relativePos = pos.xyz - pivot;
                    
                    // 应用旋转
                    float3 rotatedPos;
                    rotatedPos.x = relativePos.x * cosAngle - relativePos.z * sinAngle + pivot.x;
                    rotatedPos.z = relativePos.x * sinAngle + relativePos.z * cosAngle + pivot.z;
                    rotatedPos.y = relativePos.y + yBend * abs(relativePos.x) * 2;
                    
                    pos.xyz = rotatedPos;
                    
                    // 判断是否翻转到背面
                    float dotProduct = dot(normalize(rotatedPos - pivot), float3(flipSide, 0, 0));
                    OUT.flipState = dotProduct > 0 ? 1 : 2; // 1=正面, 2=背面
                    
                    OUT.curvature = curvatureAmount;
                }
                else
                {
                    OUT.flipState = isRightPage + (isLeftPage * 0); // 右页=1, 左页=0
                    OUT.curvature = 0;
                }
                
                OUT.side = isRightPage;
                OUT.vertex = UnityObjectToClipPos(pos);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float angle = radians(_FlipAngle);
                float flipProgress = sin(angle * 0.5);
                float flipSide = _FlipSide;
                
                half4 color;
                
                // 检查是否在书脊区域
                float centerX = 0.5;
                float isBinding = step(centerX - _BindingWidth, IN.texcoord.x) * 
                                 step(IN.texcoord.x, centerX + _BindingWidth);
                
                if (isBinding > 0.5)
                {
                    // 书脊区域显示书脊颜色
                    color = _BindingColor;
                    
                    // 添加一些阴影效果使书脊更有立体感
                    float bindingShade = 1 - abs(IN.texcoord.x - centerX) / _BindingWidth * 0.3;
                    color.rgb *= bindingShade;
                }
                else
                {
                    // 非书脊区域
                    bool isRightPage = IN.texcoord.x > centerX + _BindingWidth;
                    bool isLeftPage = IN.texcoord.x < centerX - _BindingWidth;
                    
                    // 计算纹理坐标（去除书脊区域的影响）
                    float2 pageUV = IN.texcoord;
                    
                    if (isRightPage)
                    {
                        // 右页UV调整
                        pageUV.x = (pageUV.x - (centerX + _BindingWidth)) / (0.5 - _BindingWidth);
                    }
                    else if (isLeftPage)
                    {
                        // 左页UV调整
                        pageUV.x = pageUV.x / (0.5 - _BindingWidth);
                    }
                    
                    // 根据翻页状态决定显示内容
                    if (IN.flipState == 2 && flipProgress > 0.01) // 背面
                    {
                        if (isRightPage)
                        {
                            // 右页背面显示左页内容（翻转）
                            float2 backUV = float2(1 - pageUV.x, pageUV.y);
                            color = tex2D(_LeftPageTexture, backUV) * _BackColor;
                        }
                        else
                        {
                            // 左页背面显示右页内容（翻转）
                            float2 backUV = float2(1 - pageUV.x, pageUV.y);
                            color = tex2D(_RightPageTexture, backUV) * _BackColor;
                        }
                    }
                    else // 正面
                    {
                        if (isRightPage)
                        {
                            color = tex2D(_RightPageTexture, pageUV);
                        }
                        else
                        {
                            color = tex2D(_LeftPageTexture, pageUV);
                        }
                    }
                    
                    // 添加阴影效果
                    float shadow = 0;
                    
                    // 在书脊附近添加阴影
                    float distanceToSpine = abs(IN.texcoord.x - centerX);
                    if (distanceToSpine < 0.1)
                    {
                        float spineShadow = (0.1 - distanceToSpine) / 0.1 * _ShadowStrength;
                        shadow += spineShadow;
                    }
                    
                    // 在翻页边缘添加阴影
                    if (IN.curvature > 0)
                    {
                        float edgeShadow = IN.curvature * _ShadowStrength * flipProgress;
                        shadow += edgeShadow;
                    }
                    
                    // 根据翻页进度添加渐变阴影
                    if (IN.flipState == 2)
                    {
                        // 背面稍微暗一些
                        shadow += 0.2 * flipProgress;
                    }
                    
                    color.rgb = color.rgb * (1 - shadow * 0.5);
                    
                    // 添加高光效果
                    if (IN.curvature > 0.1 && flipProgress > 0.3)
                    {
                        float highlight = sin(IN.texcoord.y * 3.14159) * IN.curvature * 0.3;
                        color.rgb += highlight;
                    }
                }
                
                color.a *= _Color.a;
                
                // UI裁剪
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}