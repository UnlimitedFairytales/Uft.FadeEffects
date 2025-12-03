Shader "Uft.FadeEffects/Legacy/SepiaPostEffect"
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
                // 元の色を取得
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 src = col.rgb;

                // セピア変換（よく使われる行列）
                float3 sepia;
                sepia.r = dot(src, float3(0.393, 0.769, 0.189));
                sepia.g = dot(src, float3(0.349, 0.686, 0.168));
                sepia.b = dot(src, float3(0.272, 0.534, 0.131));

                // _Amount で元色 <-> セピアを補間
                col.rgb = lerp(src, sepia, _Amount);

                return col;
            }
            ENDCG
        }
    }
}