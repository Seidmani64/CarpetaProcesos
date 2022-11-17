Shader "Custom/SimpleDiffuse"
{
    Properties
    {
       _MainTex ("Texture", 2D) = "white" {}
       _LightInt ("Light intensity", Range(0, 1)) = 1
       _Ambient ("Ambient", Range(0, 1)) = 1 
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "LightMode" = "ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
                #include "UnityCG.cginc"

                #pragma vertex vert
                #pragma fragment frag

                sampler2D _MainTex;
                float4 _MainTex_ST;

                float _LightInt;
                float _Ambient;

                float4 _LightColor0;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal: NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 normal_world: TEXCOORD1;
                };

                float3 lambert_diffuse(float3 light_color, float light_int, float3 normal, float3 light_dir)
                {
                    return light_color * light_int * max(0, dot(normal, light_dir));
                }

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex =  UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    o.normal_world = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0))).xyz;

                    return o;
                }

                float4 frag (v2f i) : SV_Target
                {
                    float4 color = tex2D(_MainTex, i.uv);
                    half3 ambient_color = ShadeSH9(float4(i.normal_world, 1.0)) * _Ambient;

                    float3 light_direction = normalize(_WorldSpaceLightPos0.xyz);
                    
                    half3 diffuse_color = lambert_diffuse(_LightColor0.rgb, _LightInt, i.normal_world, light_direction);

                    half3 final_light = ambient_color + diffuse_color;

                    float4 final_color = color;
                    
                    final_color.rgb *= final_light;

                    return final_color;
                }

            ENDCG
        }
    }
}