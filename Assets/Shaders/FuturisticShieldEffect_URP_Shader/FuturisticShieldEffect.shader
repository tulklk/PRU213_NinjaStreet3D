Shader "Shader Graphs/FuturisticShieldEffect"
{
    Properties
    {
        _Color ("Shield Color", Color) = (0.2, 0.8, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 3
        _FresnelPower ("Fresnel Power", Float) = 6
        _PulseSpeed ("Pulse Speed", Float) = 2
        _Alpha ("Alpha", Range(0,1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 300

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
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            float4 _Color;
            float _GlowIntensity;
            float _FresnelPower;
            float _PulseSpeed;
            float _Alpha;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float time = _Time.y * _PulseSpeed;
                float fresnel = pow(1.0 - saturate(dot(IN.viewDirWS, IN.normalWS)), _FresnelPower);
                float pulse = sin(time * 6.2831) * 0.5 + 0.5;
                float alpha = saturate(_Alpha + fresnel * 0.4 + pulse * 0.2);

                float3 glow = _Color.rgb * (_GlowIntensity + fresnel * 2.0 + pulse);
                return float4(glow, alpha);
            }
            ENDHLSL
        }
    }
}
