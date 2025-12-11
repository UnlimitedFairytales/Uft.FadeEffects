Shader "Uft.FadeEffects/Urp/RuleFade"
{
    Properties
    {
        _Amount      ("Amount",      Range(0, 1)) = 0
        _Softness    ("Softness",    Range(0, 1)) = 0.5
        _SubTex      ("Sub Texture",          2D) = "white" {}
        _SubTexColor ("Sub Texture Color", Color) = (1,1,1,1)
        _RuleTex     ("Rule Texture",         2D) = "gray"  {}
        [IntRange]
        _Invert      ("Invert",      Range(0, 1)) = 0
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
                float _Softness;
                float4 _SubTexColor;
                float _Invert;
            CBUFFER_END

            TEXTURE2D_X(_SubTex);
            SAMPLER(sampler_SubTex);
            TEXTURE2D_X(_RuleTex);
            SAMPLER(sampler_RuleTex);

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                // 元の色を取得
                half4 fromCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                half4 toCol   = SAMPLE_TEXTURE2D_X(_SubTex,      sampler_SubTex,      uv) * _SubTexColor;
                float rule    = SAMPLE_TEXTURE2D_X(_RuleTex,     sampler_RuleTex,     uv).r;

                // ルールをネガポジ反転するかどうかのオプション
                rule = lerp(rule, 1.0 - rule, _Invert);

                // ルールフェード計算 (Amountに応じてSoftnessの影響は三角形。0.5でそのまま。)
                float t;
                float s = _Softness * (1.0 - abs(2.0 * _Amount - 1.0));
                if (s <= 0)
                    t = step(rule, _Amount);
                else
                {
                    t = smoothstep(rule - s, rule + s, _Amount);
                }
                return lerp(fromCol, toCol, t);
            }
            ENDHLSL
        }
    }
}