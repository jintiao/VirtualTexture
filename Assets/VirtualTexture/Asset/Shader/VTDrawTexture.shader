Shader "Hidden/VirtualTexture/DrawTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;	
            float4x4 _ImageMVP;

            v2f_img vert (appdata_img v)
            {
                v2f_img o;
                o.pos = mul(_ImageMVP, v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            fixed4 frag (v2f_img i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
