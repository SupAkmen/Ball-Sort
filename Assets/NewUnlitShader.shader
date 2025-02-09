Shader "Unlit/NewUnlitShader"
{
   Properties
    {
        _TopColor ("Top Color", Color) = (0, 1, 1, 1)
        _BottomColor ("Bottom Color", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float gradient = i.uv.y; // Gradient theo trục Y
                return lerp(_BottomColor, _TopColor, gradient);
            }
            ENDCG
        }
    }
}
