Shader "Custom/GlowShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Range(0, 2)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.3
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.0
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.3
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        half4 _Color;
        half4 _EmissionColor;
        half _EmissionStrength;
        half _Glossiness;
        half _Metallic;
        half _PulseSpeed;
        half _PulseAmount;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Główny kolor
            o.Albedo = _Color.rgb;
            
            // Metallic i smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            // Pulsujący efekt glow
            float pulse = sin(_Time.y * _PulseSpeed) * _PulseAmount + 1.0;
            o.Emission = _EmissionColor.rgb * _EmissionStrength * pulse;
            
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    
    FallBack "Standard"
}
