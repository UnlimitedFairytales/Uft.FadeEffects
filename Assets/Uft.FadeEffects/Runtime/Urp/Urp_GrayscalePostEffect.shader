Shader "Uft.FadeEffects/Urp/GrayscalePostEffect"
{
    Properties
    {
        _Amount ("Amount", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        // ポストエフェクトのお約束
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            // URP対応は、UnityCG.cginc vert_img から Blit.hlsl Vert に変更で対応
            #pragma vertex Vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Amount;
            CBUFFER_END

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // 明度計算（YCbCr系）
                half gray   = dot(col.rgb, half3(0.299, 0.587, 0.114));

                // _Amount で元色 <-> グレースケールを補間
                half amount = saturate((half)_Amount);
                col.rgb = lerp(col.rgb, gray.xxx, amount);
                return col;
            }
            ENDHLSL
        }
    }
}