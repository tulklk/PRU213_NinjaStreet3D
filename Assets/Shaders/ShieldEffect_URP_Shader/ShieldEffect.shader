Shader "Shader Graphs/ShieldEffect"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0.75, 1, 1)
        _NoiseScale ("Noise Scale", Float) = 5
        _Speed ("Speed", Float) = 2
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
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float _NoiseScale;
            float _Speed;
            float4 _Color;
            float _Alpha;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float time = _Time.y * _Speed;
                float2 uv = IN.uv * _NoiseScale;
                float noise = sin(uv.x + time) * cos(uv.y + time);
                float fresnel = saturate(dot(normalize(IN.normalWS), float3(0, 0, 1)));
                float alpha = saturate(_Alpha + noise * 0.2 + fresnel * 0.5);
                return float4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
