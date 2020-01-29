Shader "PostEffects/Averaging"
{
    Properties
    {
        _MainTex("Base", 2D) = ""{}
        // _Filter("Filter", 2D) = ""{}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _PixelSize;

    float4 frag(v2f_img i) : SV_Target
    {
        
        float2 pixelSize = _ScreenParams.zw - 1;
        float4 color = float4(0, 0, 0, 0);
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(-1, -1)) * 1 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(0, -1)) * 2 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(1, -1)) * 1 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(-1, 0)) * 2 / 16;
        color += tex2D(_MainTex, i.uv) * 4 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(1, 0)) * 2 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(-1, 1)) * 1 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(0, 1)) * 2 / 16;
        color += tex2D(_MainTex, i.uv + _PixelSize * float2(1, 1)) * 1 / 16;

        return color;
    }

    ENDCG

    SubShader
    {
        pass
        {
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