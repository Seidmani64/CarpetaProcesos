Shader "Custom/SimpleColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Texture Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
                #include "UnityCG.cginc"

                #pragma vertex vert
                #pragma fragment frag

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex =  UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    return o;
                }

                float4 frag (v2f i) : SV_Target
                {
                    float4 color = tex2D(_MainTex, i.uv);
                    return color * _Color;
                }
            ENDCG
        }
    }
}