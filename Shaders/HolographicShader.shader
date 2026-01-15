Shader "Custom/HolographicShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0, 1, 1, 1)
        _ScanlineSpeed ("Scanline Speed", Range(0, 10)) = 2.0
        _ScanlineWidth ("Scanline Width", Range(0, 1)) = 0.1
        _Transparency ("Transparency", Range(0, 1)) = 0.5
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        _GlitchAmount ("Glitch Amount", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        struct Input
        {
            float3 viewDir;
            float3 worldPos;
        };

        fixed4 _Color;
        half _ScanlineSpeed;
        half _ScanlineWidth;
        half _Transparency;
        half _RimPower;
        half _GlitchAmount;

        // Funkcja noise
        float random(float2 p)
        {
            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Efekt skanowania (scanlines)
            float scanline = frac(IN.worldPos.y * 10.0 - _Time.y * _ScanlineSpeed);
            float scanlineEffect = smoothstep(_ScanlineWidth, 0.0, abs(scanline - 0.5) * 2.0);
            
            // Efekt rim (świecenie na krawędziach)
            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower);
            
            // Efekt glitch (losowe zakłócenia)
            float glitch = random(floor(IN.worldPos.xy * 10.0 + _Time.y)) * _GlitchAmount;
            
            // Kombinacja efektów
            fixed3 finalColor = _Color.rgb * (1.0 + scanlineEffect * 0.5 + rim);
            finalColor += glitch;
            
            o.Albedo = finalColor;
            o.Emission = finalColor * rim;
            o.Alpha = _Transparency * (0.5 + scanlineEffect * 0.5 + rim * 0.5);
        }
        ENDCG
    }
    
    FallBack "Transparent/Diffuse"
}
