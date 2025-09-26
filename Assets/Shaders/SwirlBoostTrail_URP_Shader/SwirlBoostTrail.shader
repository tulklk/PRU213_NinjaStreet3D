Shader "Shader Graphs/SwirlBoostTrail"
{
    Properties
    {
        _Color ("Trail Color", Color) = (0, 1, 1, 1)
        _Speed ("Swirl Speed", Float) = 4
        _SwirlStrength ("Swirl Strength", Float) = 10
        _Alpha ("Alpha", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Speed;
            float _SwirlStrength;
            float _Alpha;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 uv = IN.uv - center;

                float angle = atan2(uv.y, uv.x);
                float radius = length(uv);
                float swirl = sin(angle * _SwirlStrength - _Time.y * _Speed);

                float alpha = saturate(_Alpha * (1 - radius * 1.5) + swirl * 0.2);
                float3 color = _Color.rgb * (0.5 + swirl * 0.5);

                return float4(color, alpha);
            }
            ENDHLSL
        }
    }
}
