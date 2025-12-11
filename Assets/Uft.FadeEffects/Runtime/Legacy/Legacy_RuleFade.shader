Shader "Uft.FadeEffects/Legacy/RuleFade"
{
    Properties
    {
        _Amount      ("Amount",      Range(0, 1)) = 0
        _Softness    ("Softness",    Range(0, 1)) = 0.5
        _MainTex     ("Texture",              2D) = "white" {}
        _SubTex      ("Sub Texture",          2D) = "white" {}
        _SubTexColor ("Sub Texture Color", Color) = (1,1,1,1)
        _RuleTex     ("Rule Texture",         2D) = "gray"  {}
        [IntRange]
        _Invert      ("Invert",      Range(0, 1))   = 0
    }

    SubShader
    {
        // ポストエフェクトのお約束
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _SubTex;
            sampler2D _RuleTex;
            float _Amount;
            float _Softness;
            float4 _SubTexColor;
            float _Invert;

            fixed4 frag (v2f_img i) : SV_Target
            {
                // 元の色を取得
                fixed4 fromCol = tex2D(_MainTex, i.uv);
                fixed4 toCol   = tex2D(_SubTex, i.uv) * _SubTexColor;
                float  rule    = tex2D(_RuleTex, i.uv).r;

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
            ENDCG
        }
    }
}