Shader "PostEffects/Binary"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _Threshold("", Range(0, 1)) = 1
        _DitherTex("", 2D) = ""{}
        _Color0("", Color) = (0, 0, 0)
        _Color1("", Color) = (1, 1, 1)
        [Gamma] _Opacity("", Range(0, 1)) = 1
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    sampler2D _DitherTex;
    float2 _DitherTex_TexelSize;

    half _Scale;
    half3 _Color0;
    half3 _Color1;
    half _Opacity;
    half _Threshold;

    half4 frag(v2f_img i) : SV_Target
    {
        half4 source = tex2D(_MainTex, i.uv);

        float2 dither_uv = i.uv * _DitherTex_TexelSize;
        dither_uv /= _MainTex_TexelSize * _Scale;
        half dither = tex2D(_DitherTex, dither_uv).a + 0.5 / 256;

        
        // Relative luminance in linear RGB space
    #ifdef UNITY_COLORSPACE_GAMMA
        half rlum = LinearRgbToLuminance(GammaToLinearSpace(saturate(source.rgb)));
    #else
        half rlum = LinearRgbToLuminance(source.rgb);
    #endif

        half3 rgb = rlum < dither * _Threshold ? _Color0 : _Color1;
        return half4(lerp(source.rgb, rgb, _Opacity), source.a);
    }

    ENDCG

    SubShader{
        Pass{
            ZTest Always Cull Off Zwrite Off

            CGPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
