Shader "Custom/HolographicShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BorderWidth ("Rim effect", Range(-1, 1)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 200
        Cull Off
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;

            float3 worldNormal;
            float3 viewDir;
        };

        float _BorderWidth;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            float border = 1 - (abs(dot(IN.viewDir, IN.worldNormal)));
            
            float alpha = (border * (1-_BorderWidth));
            o.Alpha = c.a * alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
