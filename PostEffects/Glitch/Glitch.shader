shader "PostEffects/Glitch"
{
    Properties
    {
        _MainTex("Base", 2D) = ""{}
        _GlitchTex("Glitch", 2D) = ""{}
        _BufferTex("Buffer", 2D) = ""{}
        _Intensity("Intensity", Float) = 1
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _GlitchTex;
    sampler2D _BufferTex;
    float _Intensity;

    float4 frag(v2f_img i) : SV_Target
    {
        float2 spc = (i.uv - 0.5) * 2.0;
        float r2 = lerp(0.1f, 1.0f, dot(spc, spc));
        float4 glitch = tex2D(_GlitchTex, i.uv);

        float thresh = 1.001 - _Intensity * 1.001;
        float w_r = step(thresh, pow(glitch.z, 2.5));
        float w_g = step(thresh, pow(glitch.w, 2.5)); 
        float w_b = step(thresh, pow(glitch.z, 3.5));


        // Displacement
        float2 uv_r = i.uv + float2((glitch.w - 0.5) * 0.1, 0) * w_r * r2;
        // float2 uv_g = i.uv + float2((glitch.z - 0.5) * 0.1, 0) * w_g * r2;
        float2 uv_b = i.uv + float2((glitch.x - 0.5) * 0.1, 0) * w_b * r2;

        float4 source = tex2D(_MainTex, i.uv);
        float4 source_r = tex2D(_MainTex, uv_r);
        float4 source_b = tex2D(_MainTex, uv_b);
        float4 buffer_r = tex2D(_BufferTex, uv_r);
        // float4 buffer_g = tex2D(_BufferTex, uv_g);
        float4 buffer_b = tex2D(_BufferTex, uv_b);
        
        
        // float3 color = source.rgb * (1.0 - _Intensity * r2) + float3(buffer_r.r, source.g, buffer_b.b)*(_Intensity * r2);
        float3 color = float3(source_r.r, source.g, source_b.b) * (1.0 - _Intensity * r2) + float3(buffer_r.r, source.g, buffer_b.b)*(_Intensity * r2);

        // float3 color = float3(buffer_r.r, source.g, buffer_b.b);

        // // Mix with buffer.
        // float w_s = step((source.r + source.g + source.b) / 3.0, buffer.r);

        // float3 color = source + lerp(0.0, lerp(0.0, buffer, w_s), w_b).rgb;
        
        // Suffle color components.
        // color = lerp(color, color * 2 - source.bbg + color.grr, w_c);

        return float4(color, source.a);
    }
    ENDCG

    Subshader
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