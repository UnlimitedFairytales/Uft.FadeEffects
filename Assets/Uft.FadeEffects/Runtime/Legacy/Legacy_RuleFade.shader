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

                // ルールフェード計算
                float t;
                if (_Softness <= 0.001)
                    t = step(rule, _Amount);
                else
                {
                    float a = lerp(-_Softness, 1.0 + _Softness, _Amount);
                    t = smoothstep(rule - _Softness, rule + _Softness, a);
                }
                return lerp(fromCol, toCol, t);
            }
            ENDCG
        }
    }
}