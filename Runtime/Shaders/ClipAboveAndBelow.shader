Shader "CP/ClipAboveAndBelow"
{
    Properties
    {
        _Floor("Cutoff floor height", Float) = 0.0
        _Ceiling("Cutoff ceiling height", Float) = 1.0
        _BaseMap("Base Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags {"RenderPipeline" = "UniversalPipeline" "Queue" = "AlphaTest"}
            LOD 200

            Pass
            {
                Name "LitPass"
                Tags {"LightMode" = "UniversalForward"}

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                    float2 uv : TEXCOORD0;
                    float4 positionHCS : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                };

                sampler2D _BaseMap;
                float4 _BaseColor;
                float _Floor;
                float _Ceiling;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = IN.uv;
                    OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz); // Convert to world space
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    // Clip if below cutoff height
                    if (IN.worldPos.y < _Floor)
                        clip(-1); // Fully discards pixel

                    if (IN.worldPos.y > _Ceiling)
                        clip(-1); // Fully discards pixel

                    float4 texColor = tex2D(_BaseMap, IN.uv) * _BaseColor;
                    return texColor;
                }
                ENDHLSL
            }
        }
}
