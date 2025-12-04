Shader "Uft.FadeEffects/Urp/SepiaPostEffect"
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

                // 元の色を取得
                half3 src = col.rgb;

                // セピア変換（よく使われる行列）
                half3 sepia;
                sepia.r = dot(src, half3(0.393, 0.769, 0.189));
                sepia.g = dot(src, half3(0.349, 0.686, 0.168));
                sepia.b = dot(src, half3(0.272, 0.534, 0.131));

                // _Amount で元色 <-> セピアを補間
                half amount = saturate((half)_Amount);
                col.rgb = lerp(src, sepia, amount);
                return col;
            }
            ENDHLSL
        }
    }
}