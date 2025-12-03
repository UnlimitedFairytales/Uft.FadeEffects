Shader "Uft.FadeEffects/Legacy/DirectionalGhostPostEffect"
{
    Properties
    {
        _Amount ("Amount", Range(0, 1)) = 0
        _Angle  ("Angle", Range(0, 360)) = 0
        _MainTex ("MainTex", 2D) = "white" {}
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
            float _Amount;
            float _Angle;
            float4 _MainTex_TexelSize;

            fixed4 frag (v2f_img i) : SV_Target
            {
                // ---------- 回転方向ベクトル ----------
                float rad = radians(_Angle);
                float2 dir = float2(cos(rad), sin(rad));

                // ブラー距離（Amount × 定数）
                float offset = _Amount * 100.0;

                float2 texel = _MainTex_TexelSize.xy;
                float2 o = dir * texel * offset;

                // ---------- サンプル ----------
                fixed4 col = tex2D(_MainTex, i.uv);

                col += tex2D(_MainTex, i.uv +  o);
                col += tex2D(_MainTex, i.uv -  o);

                // 平均
                col *= 1.0 / 3.0;

                return col;
            }
            ENDCG
        }
    }
}