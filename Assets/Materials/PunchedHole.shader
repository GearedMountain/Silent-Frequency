Shader "Custom/PunchThroughTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _HoleRadius ("Hole Radius", Float) = 1.0
        _HoleCount ("Hole Count", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float4 _HoleCenters[64]; // Up to 64 hole punchers
            float _HoleRadius;
            float _HoleCount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 pos = i.worldPos;
                bool insideHole = false;

                for (int idx = 0; idx < 64; idx++)
                {
                    if (idx >= _HoleCount) break;
                    float3 holePos = _HoleCenters[idx].xyz;
                    float dist = distance(pos, holePos);
                    if (dist < _HoleRadius)
                    {
                        insideHole = true;
                        break;
                    }
                }

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                if (insideHole)
                    col.a = 0;

                return col;
            }
            ENDCG
        }
    }
}
