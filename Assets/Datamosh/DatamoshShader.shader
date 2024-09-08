// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/DatamoshShader"
{
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline"="Universal"
        }
        LOD 100

        ZWrite Off Cull Off
        Pass
        {
            Name "DatamoshPass"
            Blend One Zero


            HLSLPROGRAM
            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"

            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            TEXTURE2D_X(_MotionVectorTexture);
            SAMPLER(sampler_MotionVectorTexture);
            SAMPLER(sampler_BlitTexture);
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            bool moving(float4 m)
            {
                float4 none = float4(0,0,0,0);
                return (m.r > 0 || m.r < 0 || m.g > 0 || m.g < 0);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
                float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

                output.positionCS = pos;
                output.texcoord = DYNAMIC_SCALING_APPLY_SCALEBIAS(uv);
                
                return output;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float2 originalTexcoord = i.texcoord;

                float4 motion = SAMPLE_TEXTURE2D_X(_MotionVectorTexture, sampler_MotionVectorTexture, i.texcoord);
                float2 shiftedTexcoord = i.texcoord + motion.rg;
                
                if (moving(motion))
                {
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, shiftedTexcoord);
                }
                else
                {
                    return SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, originalTexcoord);
                }
            }
            ENDHLSL
        }

        Pass
        {
            Name "SimpleBlit"

            HLSLPROGRAM
            //#include "UnityCG.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"

            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            SAMPLER(sampler_BlitTexture);

            float4 frag (Varyings i) : SV_Target
            {
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, i.texcoord);
            }
            ENDHLSL
        }
    }
}
