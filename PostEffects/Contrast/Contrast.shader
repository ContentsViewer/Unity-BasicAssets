Shader "PostEffects/Contrast"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Contrast ("Contrast", Range(1, 20)) = 10
    }
    SubShader
    {
        Pass{
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert_img
            #pragma fragment frag

            sampler2D _MainTex;
            float _Contrast;

            float4 frag(v2f_img i) : SV_Target{
                float4 c = tex2D(_MainTex, i.uv);
                
                // if(i.uv.x > 0.5){
                c = 1 / (1 + exp(-_Contrast*(c-0.5)));
                // }

                return c;
            }
            ENDCG
        }
    }
}
