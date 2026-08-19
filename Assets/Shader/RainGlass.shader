Shader "UI/RainGlass"

{

  Properties

  {

    [PerRendererData] [HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}

    _Color("Tint", Color) = (1,1,1,1)

    [Toggle] _CheapNormals("Use Cheap Normals", Float) = 0

      // 行为与强度控制（精简）

      _TimeScale("Time Scale", Range(0,3)) = 1

      _Speed("Drop Speed", Range(0,3)) = 1

      _RainAmount("Rain Amount", Range(0,1)) = 0.8

      _NormalScale("Normal Scale", Range(0,3)) = 1

      _DropScale("Drop Scale", Range(0.5,5)) = 1

      _Blur("Blur Radius", Range(0,12)) = 6

      _GlassTintColor("Glass Tint Color", Color) = (1,1,1,1)

      _GlassTintStrength("Glass Tint Strength", Range(0,1)) = 0.1

      // 玻璃可见性增强（可调）：高光/边缘/色调

      [Toggle] _UseSpecular("Use Specular/Rim", Float) = 1

      _SpecularColor("Specular Color", Color) = (1,1,1,1)

      _SpecularIntensity("Specular Intensity", Range(-2,2)) = 0.6

      _SpecularPower("Specular Power", Range(4,64)) = 16

      _LightDir("Light Dir (xy)", Vector) = (-0.5,0.5,0,0)

      _RimIntensity("Rim Intensity", Range(0,2)) = 0.5

      _RimWidth("Rim Width", Range(0,1)) = 0.25



      // UI 默认遮罩/裁剪属性（与 UI/Default 保持一致）

      [PerRendererData][HideInInspector] _StencilComp("Stencil Comparison", Float) = 8

      [PerRendererData][HideInInspector] _Stencil("Stencil ID", Float) = 0

      [PerRendererData][HideInInspector] _StencilOp("Stencil Operation", Float) = 0

      [PerRendererData][HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255

      [PerRendererData][HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255

      [PerRendererData][HideInInspector] _ColorMask("Color Mask", Float) = 15

      [PerRendererData][HideInInspector] _ClipRect("Clip Rect", Vector) = (-10000,-10000,10000,10000)

      [PerRendererData][HideInInspector] _TextureSampleAdd("TextureSampleAdd", Vector) = (0,0,0,0)

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

          float4 vertex : POSITION;

          float4 color : COLOR;

          float2 texcoord : TEXCOORD0;

        };



        struct v2f

        {

          float4 vertex : SV_POSITION;

          float2 uv : TEXCOORD0;

          float4 color : COLOR;

          float2 worldPos : TEXCOORD1; // 用于 UI 剪裁

          float4 screenPos : TEXCOORD2; // 屏幕坐标（用于采样相机不透明纹理）

        };



        sampler2D _MainTex;

        float4 _MainTex_TexelSize;

        sampler2D _CameraOpaqueTexture;

        float4 _CameraOpaqueTexture_TexelSize;

        float4 _Color;

        float4 _ClipRect;

        float4 _TextureSampleAdd;

        float _CheapNormals;

        float _TimeScale;

        float _Speed;

        float _RainAmount;

        float _Blur;

        float _NormalScale;

        float _DropScale;

        float _UseSpecular;

        float4 _SpecularColor;

        float _SpecularIntensity;

        float _SpecularPower;

        float4 _LightDir; // xy 

        float4 _GlassTintColor;

        float _GlassTintStrength;

        float _RimIntensity;

        float _RimWidth;



        v2f vert(appdata_t v)

        {

          v2f o;

          o.vertex = UnityObjectToClipPos(v.vertex);

          o.uv = v.texcoord;

          o.color = v.color * _Color;

          o.worldPos = v.vertex.xy;

          o.screenPos = ComputeScreenPos(o.vertex);

          return o;

        }



        // smoothstep 封装

        float S(float a, float b, float t)

        {

          float s = saturate((t - a) / (b - a));

          return s * s * (3.0 - 2.0 * s);

        }



        float3 N13(float p)

        {

          float3 p3 = frac(float3(p, p, p) * float3(0.1031, 0.11369, 0.13787));

          p3 += dot(p3, p3.yzx + 19.19);

          return frac(float3((p3.x + p3.y) * p3.z, (p3.x + p3.z) * p3.y, (p3.y + p3.z) * p3.x));

        }

        float N(float t)

        {

          return frac(sin(t * 12345.564) * 7658.76);

        }



        float Saw(float b, float t)

        {

          return S(0., b, t) * S(1., b, t);

        }



        float2 DropLayer2(float2 uv, float t)

        {

          float2 UV = uv;



          uv.y += t * 0.75;

          float2 a = float2(6., 1.);

          float2 grid = a * 2.;

          float2 id = floor(uv * grid);



          float colShift = N(id.x);

          uv.y += colShift;



          id = floor(uv * grid);

          float3 n = N13(id.x * 35.2 + id.y * 2376.1);

          float2 st = frac(uv * grid) - float2(.5, 0);



          float x = n.x - .5;



          float y = UV.y * 20.;

          float wiggle = sin(y + sin(y));

          x += wiggle * (.5 - abs(x)) * (n.z - .5);

          x *= .7;

          float ti = frac(t + n.z);

          y = (Saw(.85, ti) - .5) * .9 + .5;

          float2 p = float2(x, y);



          float d = length((st - p) * a.yx);



          float mainDrop = S(.4, .0, d);



          float r = sqrt(S(1., y, st.y));

          float cd = abs(st.x - x);

          float trail = S(.23 * r, .15 * r * r, cd);

          float trailFront = S(-.02, .02, st.y - y);

          trail *= trailFront * r * r;



          y = UV.y;

          float trail2 = S(.2 * r, .0, cd);

          float droplets = max(0., (sin(y * (1. - y) * 120.) - st.y)) * trail2 * trailFront * n.z;

          y = frac(y * 10.) + (st.y - .5);

          float dd = length(st - float2(x, y));

          droplets = S(.3, 0., dd);

          float m = mainDrop + droplets * r * trailFront;



          return float2(m, trail);

        }



        float StaticDrops(float2 uv, float t)

        {

          uv *= 40.;



          float2 id = floor(uv);

          uv = frac(uv) - .5;

          float3 n = N13(id.x * 107.45 + id.y * 3543.654);

          float2 p = (n.xy - .5) * .7;

          float d = length(uv - p);



          float fade = Saw(.025, frac(t + n.z));

          float c = S(.3, 0., d) * frac(n.z * 10.) * fade;

          return c;

        }



        float2 Drops(float2 uv, float t, float l0, float l1, float l2)

        {

          float s = StaticDrops(uv, t) * l0;

          float2 m1 = DropLayer2(uv, t) * l1;

          float2 m2 = DropLayer2(uv * 1.85, t) * l2;



          float c = s + m1.x + m2.x;

          c = S(.3, 1., c);



          return float2(c, max(m1.y * l0, m2.y * l1));

        }



        // 自适应模糊：小半径使用十字5点，大半径使用9点

        half3 SampleBlurAdaptive(sampler2D tex, float2 uv, float2 texelSize, half radius)

        {

          float2 o = texelSize * radius;

          half3 c = 0;

          UNITY_BRANCH if (radius <= 2.0)

          {

            // 5 点十字采样（中心 + 上下左右），权重调整以近似 9 点的亮度

            c += tex2D(tex, uv).rgb * 0.34;

            c += tex2D(tex, uv + float2(o.x, 0)).rgb * 0.165;

            c += tex2D(tex, uv + float2(-o.x, 0)).rgb * 0.165;

            c += tex2D(tex, uv + float2(0, o.y)).rgb * 0.165;

            c += tex2D(tex, uv + float2(0, -o.y)).rgb * 0.165;

            return c;

          }

          // 9 点采样（中心 + 十字 + 四角）

          c += tex2D(tex, uv).rgb * 0.24;

          c += tex2D(tex, uv + float2(o.x, 0)).rgb * 0.11;

          c += tex2D(tex, uv + float2(-o.x, 0)).rgb * 0.11;

          c += tex2D(tex, uv + float2(0, o.y)).rgb * 0.11;

          c += tex2D(tex, uv + float2(0, -o.y)).rgb * 0.11;

          c += tex2D(tex, uv + float2(o.x, o.y)).rgb * 0.08;

          c += tex2D(tex, uv + float2(o.x, -o.y)).rgb * 0.08;

          c += tex2D(tex, uv + float2(-o.x, o.y)).rgb * 0.08;

          c += tex2D(tex, uv + float2(-o.x, -o.y)).rgb * 0.08;

          return c;

        }



        fixed4 frag(v2f IN) : SV_Target

        {

          // 使用屏幕尺寸作为 iResolution

          float2 iRes = _ScreenParams.xy; // 屏幕宽高（像素）

          float2 UV = IN.screenPos.xy / IN.screenPos.w; // 0..1 屏幕坐标

          UV = saturate(UV);

          float2 fragCoord = UV * iRes;

          float2 uv = (fragCoord - 0.5 * iRes) / iRes.y; // 以高度归一化的中心坐标



          // 统一时间

          float T = _Time.y * _TimeScale;

          float t = T * 0.2 * _Speed;



          // 雨量控制

          half rainAmount = _RainAmount;



          // 模糊控制

          half blurBase = _Blur;

          float staticDrops = S(-.5, 1., rainAmount) * 2.;

          float layer1 = S(.25, .75, rainAmount);

          float layer2 = S(.0, .5, rainAmount);

          // 水滴大小控制：缩放水滴模式的空间频率

          float2 dropUV = uv / max(0.0001, _DropScale);

          float2 c = Drops(dropUV, t, staticDrops, layer1, layer2);

          float2 n;

          UNITY_BRANCH if (_CheapNormals > 0.5)

          {

            n = float2(ddx(c.x), ddy(c.x));

          }

          else

          {

            float2 e = float2(.001, 0.);

            float cx = Drops(dropUV + e, t, staticDrops, layer1, layer2).x;

            float cy = Drops(dropUV + e.yx, t, staticDrops, layer1, layer2).x;

            n = float2(cx - c.x, cy - c.x);

          }

          // 法线强度控制

          n *= _NormalScale;

          // 自适应模糊：雨滴内部更清晰（略减小模糊半径），其余区域使用统一模糊

          half sharpMask = S(.1, .2, c.x);

          half blurRadius = blurBase * (1.0 - 0.6 * sharpMask);

          // 在相机不透明纹理上进行 9 点模糊采样来近似 LOD 模糊（若不可用则回退到 _MainTex）

          float2 sampleUV = UV + float2(n.x, -n.y); //上半采样天空、下半采样草地

          float hasCam = (_CameraOpaqueTexture_TexelSize.x + _CameraOpaqueTexture_TexelSize.y) > 0 ? 1.0 : 0.0;

          float2 texelSize = hasCam > 0.5 ? _CameraOpaqueTexture_TexelSize.xy : float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

          half3 colCam = SampleBlurAdaptive(_CameraOpaqueTexture, sampleUV, texelSize, blurRadius);

          half3 colSprite = SampleBlurAdaptive(_MainTex, IN.uv, _MainTex_TexelSize.xy, blurRadius);

          half3 col = lerp(colCam, colSprite, 1.0 - hasCam);



          float dropMask = 1; //saturate(c.x);

          // 玻璃可见性增强（可调）：高光与边缘强调

          UNITY_BRANCH if (_UseSpecular > 0.5 && dropMask > 0.001)

          {

            float3 normal = normalize(float3(-n.x, -n.y, 1.0));

            float3 viewDir = float3(0, 0, 1);

            float3 lightDir = normalize(float3(_LightDir.x, _LightDir.y, 0.5));

            float3 H = normalize(lightDir + viewDir);

            float spec = pow(saturate(dot(normal, H)), _SpecularPower) * _SpecularIntensity;

            float edge = saturate(length(n) * (50.0 * max(0.001, (1.0 - _RimWidth))));

            float rim = edge * _RimIntensity;

            float3 specCol = _SpecularColor.rgb * (spec + rim) * dropMask;

            col += specCol;

          }

          // 玻璃色调：在水滴区域混入少量色调

          col = lerp(col, _GlassTintColor.rgb, _GlassTintStrength * dropMask);

          col = saturate(col);



          float4 outCol = float4(col, 1.0);

          outCol *= IN.color; // UI 颜色混合



          #ifdef UNITY_UI_CLIP_RECT

          float2 clipUV = UnityGet2DClipping(IN.worldPos, _ClipRect);

          outCol.a *= clipUV.x * clipUV.y;

          #endif



          clip(outCol.a - 0.001);

          return outCol;

        }

        ENDCG

      }

    }

      FallBack "UI/Default"

}