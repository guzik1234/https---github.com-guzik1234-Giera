Shader "Custom/BrickGlowShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FresnelPower ("Fresnel Power", Range(0.1, 5)) = 2.0
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2.0
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
            float3 viewDir;
            float3 worldPos;
        };

        fixed4 _Color;
        fixed4 _EmissionColor;
        half _EmissionStrength;
        half _Glossiness;
        half _Metallic;
        half _FresnelPower;
        fixed4 _FresnelColor;
        half _PulseSpeed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Podstawowy kolor
            o.Albedo = _Color.rgb;
            
            // Metallic i smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            // Efekt Fresnela (świecenie na krawędziach)
            half fresnel = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            fresnel = pow(fresnel, _FresnelPower);
            
            // Pulsujące emission
            half pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5);
            
            // Kombinacja emission z fresnel i pulsem
            o.Emission = _EmissionColor.rgb * _EmissionStrength * pulse + 
                         _FresnelColor.rgb * fresnel * 0.5;
            
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    
    FallBack "Diffuse"
}
