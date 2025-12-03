Shader "Uft.FadeEffects/Legacy/GrayscalePostEffect"
{
    Properties
    {
        _Amount ("Amount", Range(0, 1)) = 0
        _MainTex ("Texture", 2D) = "white" {}
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

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 明度計算（YCbCr系）
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // カラー → グレースケールの補間
                col.rgb = lerp(col.rgb, gray.xxx, _Amount);

                return col;
            }
            ENDCG
        }
    }
}