Shader "GOL/Grid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridColor ("Grid Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _Resolution ("Resolution", Float) = 512
        _Thickness ("Thickness", Range(0, 10)) = 5.0
        [Toogle] _ShowGrid ("ShowGrid", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _GridColor;
            float _Resolution;
            float _Thickness;
            float _ShowGrid;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 cellColor = tex2D(_MainTex, i.uv);

                if (_ShowGrid < 0.5)
                    return cellColor;

                float2 gridUV = i.uv * _Resolution;
                // 一个屏幕像素对应的gridUV
                float2 pixelSize = fwidth(gridUV);
                // 计算到grid边界的gridUV距离
                float2 distToEdge = abs(frac(gridUV - 0.5) - 0.5);
                // 将gridUV距离转换为屏幕像素距离
                float2 pixelDist = distToEdge / pixelSize;

                float lineWeight = min(pixelDist.x, pixelDist.y);

                float halfWidth = _Thickness * 0.5;

                float isLine = 1.0 - smoothstep(halfWidth - 0.5, halfWidth + 0.5, lineWeight);
                return lerp(cellColor, _GridColor, isLine);
            }
            ENDCG
        }
    }
}
