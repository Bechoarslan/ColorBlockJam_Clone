Shader "Custom/ClipPlaneUnlit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}

        _ClipPlanePos ("Clip Plane Position", Vector) = (0,0,0,0)
        _ClipPlaneNormal ("Clip Plane Normal", Vector) = (0,0,1,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;

            float3 _ClipPlanePos;
            float3 _ClipPlaneNormal;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float side = dot(_ClipPlaneNormal, i.worldPos - _ClipPlanePos);

                // Kapının arkasındaysa pixel çizilmez
                clip(side);

                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                return col;
            }
            ENDCG
        }
    }
}
