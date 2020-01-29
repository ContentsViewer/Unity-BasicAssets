Shader "PostEffects/Pixelation"
{
    Properties{
        _MainTex("Base", 2D) = ""{}
        _PixelNumberX("Pixel Number X", float) = 200
        _PixelNumberY("Pixel Number Y", float) = 200
    }


    CGINCLUDE
    #include "UnityCG.cginc"
    sampler2D _MainTex;
    float _PixelNumberX;
    float _PixelNumberY;

    float4 frag(v2f_img i) : SV_Target{
        half ratioX = 1 / _PixelNumberX;
        half ratioY = 1 / _PixelNumberY;

        half2 uv = half2((int)(i.uv.x / ratioX) * ratioX, (int)(i.uv.y / ratioY) * ratioY);
        uv.x += ratioX / 2;
        uv.y += ratioY / 2;
        return tex2D(_MainTex, uv);
    }

    ENDCG

    SubShader
    {
        Pass{
            ZTest Always Cull off Zwrite off
            Fog{ Mode off }
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
