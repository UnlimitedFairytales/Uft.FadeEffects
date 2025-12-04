Shader "Uft.FadeEffects/Urp/DirectionalGhostPostEffect"
{
    Properties
    {
        _Amount ("Amount", Range(0, 1)) = 0
        _Angle  ("Angle", Range(0, 360)) = 0
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
                float _Angle;
            CBUFFER_END

            half4 frag (Varyings input) : SV_Target
            {
                // ---------- 回転方向ベクトル ----------
                float rad = radians(_Angle);
                float2 dir = float2(cos(rad), sin(rad));

                // ブラー距離（Amount × 定数）
                float offset = _Amount * 100.0;

                float2 texel = _BlitTexture_TexelSize.xy;
                float2 o = dir * texel * offset;

                // ---------- サンプル ----------
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv +  o);
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv -  o);

                // 平均
                col *= 1.0 / 3.0;

                return col;
            }
            ENDHLSL
        }
    }
}