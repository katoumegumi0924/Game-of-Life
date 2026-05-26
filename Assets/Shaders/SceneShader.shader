Shader "GOL/SceneShader"
{
    Properties
    {
        _AliveColor ("Alive Color", Color) = (1, 1, 1, 1)
        _DeadColor ("Dead Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            StructuredBuffer<int> _CellBuffer;

            float2 _Resolution;
            float4 _AliveColor;
            float4 _DeadColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                uint x = (uint)(i.uv.x * _Resolution.x);
                uint y = (uint)(i.uv.y * _Resolution.y);

                x = min(x, (uint)_Resolution.x - 1);
                y = min(y, (uint)_Resolution.y - 1);

                uint index = y * (uint)_Resolution.x + x;

                int state = _CellBuffer[index];

                return state == 0 ? _AliveColor : _DeadColor;
            }
            ENDCG
        }
    }
}
