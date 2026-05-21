Shader "GOL/Paint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MousePos ("Mouse Position", Vector) = (0,0,0,0)
         _PaintColor ("Paint Color", Float) = 1.0
        _ResolutionX ("ResolutionX", Float) = 256
        _ResolutionY ("ResolutionY", Float) = 256
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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

            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MousePos;
            float _PaintColor;

            float _ResolutionX;
            float _ResolutionY;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 resolution =
                    float2(_ResolutionX, _ResolutionY);

                int2 mouseCell =
                    (int2)floor(_MousePos.xy * resolution);

                int2 currentCell =
                    (int2)floor(i.uv * resolution);

                if(all(mouseCell == currentCell))
                {
                    return fixed4(
                        _PaintColor,
                        _PaintColor,
                        _PaintColor,
                        1
                    );
                }

                return col;
            }
            ENDCG
        }
    }
}
