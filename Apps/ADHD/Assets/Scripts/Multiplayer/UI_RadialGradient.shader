Shader "Unlit/UI_RadialGradient"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Base Color", Color) = (1, 0.843, 0, 1) // Gold color
        _Radius ("Radius", Float) = 0.5 // Gradient size
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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

            float _Radius;
            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1; // Center the UV coordinates (-1 to 1)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.uv); // Distance from the center
                float alpha = 1.0 - smoothstep(0, _Radius, dist); // Gradient falloff
                return fixed4(_Color.rgb, alpha * _Color.a); // Apply alpha
            }
            ENDCG
        }
    }
}
