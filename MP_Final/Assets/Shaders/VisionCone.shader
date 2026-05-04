Shader "Beavergeance/VisionCone"
{
    Properties
    {
        _BaseColor  ("Base Color",  Color)       = (1, 0.1, 0.1, 0.55)
        _TipAlpha   ("Tip Alpha",   Range(0,1))  = 0.0
        _RimPower   ("Rim Power",   Range(0.5, 8)) = 2.5
        _RimStrength("Rim Strength",Range(0, 1))  = 0.4
    }

    SubShader
    {
        // URP transparent, renders on top of opaque geometry but respects depth
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
        }

        Pass
        {
            Name "VisionConeForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            // Double-sided: player can see the cone from any angle
            Cull Off

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half  _TipAlpha;
                half  _RimPower;
                half  _RimStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv         = IN.uv;
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS  = normalize(GetCameraPositionWS() - worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // uv.y: 0 = base of cylinder (near worker), 1 = tip (far end)
                // Fade alpha from base (full) -> tip (zero)
                half lengthFade = lerp(_BaseColor.a, _TipAlpha, IN.uv.y);

                // Rim: edges of cone glow more strongly — makes the cone edge crisp
                half3 N = normalize(IN.normalWS);
                half  NdotV = saturate(dot(N, normalize(IN.viewDirWS)));
                half  rim   = pow(1.0 - NdotV, _RimPower) * _RimStrength;

                half4 col = _BaseColor;
                col.a = saturate(lengthFade + rim);

                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
