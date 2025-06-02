Shader "Custom/SampleDepthUV"
{
    Properties
    {
        _MainTex("Depth Texture", 2D) = "white" {}
        _UV("UV", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _UV;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                o.pos = float4(0, 0, 0, 1); // Fullscreen quad not needed
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float d = tex2D(_MainTex, _UV.xy).r;
                return float4(d, 0, 0, 1);
            }
            ENDCG
        }
    }
}
