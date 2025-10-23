Shader "URP/Particles/Ice"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Bottom Color", Color) = (0.023, 0.205, 1, 1)
        _UpColor("Top Color", Color) = (0.45, 0.73, 1, 1)
        _ColorPosition("Color Blend Position", Range(0,1)) = 0.35
        [HDR] _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower("Fresnel Power", Float) = 6
        _FresnelScale("Fresnel Scale", Float) = 1
        _Emission("Emission Multiplier", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
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
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float4 _UpColor;
            float _ColorPosition;

            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelScale;

            float _Emission;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                OUT.color = IN.color;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normal = normalize(IN.normalWS);
                float NdotV = saturate(dot(normal, IN.viewDirWS));
                float fresnel = _FresnelScale * pow(1.0 - NdotV, _FresnelPower);

                float yBlend = saturate(IN.normalWS.y + (_ColorPosition - 1.0));
                float4 blendColor = lerp(_Color, _UpColor, yBlend);

                float4 baseTex = tex2D(_MainTex, IN.uv);
                float4 finalColor = (baseTex * blendColor * (1 - fresnel) + _FresnelColor * fresnel) * IN.color;

                float3 emission = finalColor.rgb * _Emission;

                return float4(emission, IN.color.a * finalColor.a);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
