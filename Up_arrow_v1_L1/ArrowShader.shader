Shader "AlphaShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            Pass
            {
                Tags {"LightMode" = "ForwardBase"}
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "Lighting.cginc"
                #include "UnityCG.cginc"

                struct a2v
                {
                    float4 vertex : POSITION;
                    float3 normal: NORMAL;
                };

                struct v2f
                {
                    float4 pos: SV_POSITION;
                    float3 worldNormal: TEXCOORD0;
                    float3 worldPos: TEXCOORD1;
                };

                fixed4 _Color;

                v2f vert(a2v v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    // 世界坐标不算投影，用于计算 inverseOf
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    return _Color;
                }
            ENDCG
            }
        }
        FallBack "Transparent/VertexLit"
}
