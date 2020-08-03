Shader "Custom/RewindShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _T("AnimationTime", Range(0,1)) = 0
        _Aspect("AspectRatio", Float) = 0
    }
    SubShader
    {
        // No culling or depth
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _T;
            float _Aspect;

            fixed4 frag (v2f i) : SV_Target
            {
                float distanceFromCenter = length((i.uv - 0.5) * float2(_Aspect, 1.0));

                float waveT = max(min(distanceFromCenter * 12 - (_T * _T * 3.14159 * 5.5 - 2), 3.14159), -3.14159);
                float wave = (1.0 + cos(waveT)) * 0.5;

                fixed4 col = tex2D(_MainTex, i.uv - (i.uv - 0.5) * wave * 0.3);

                return col * (0.9 + 0.1 * (1.0 - cos(waveT + 3.14159 * 0.5)));
            }
            ENDCG
        }
    }
}
