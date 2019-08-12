Shader "Hidden/VirtualTexture/FeedbackDownScale"
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
			#include "VTFeedback.cginc"	
            #pragma vertex VTVert
            #pragma fragment frag
            fixed4 frag (VTV2f i) : SV_Target
            {
                return GetMaxFeedback(i.uv, 2);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
			#include "VTFeedback.cginc"	
            #pragma vertex VTVert
            #pragma fragment frag
			fixed4 frag(VTV2f i) : SV_Target
			{
				return GetMaxFeedback(i.uv, 4);
			}
            ENDCG
        }

        Pass
        {
            CGPROGRAM
			#include "VTFeedback.cginc"	
            #pragma vertex VTVert
            #pragma fragment frag
			fixed4 frag(VTV2f i) : SV_Target
			{
				return GetMaxFeedback(i.uv, 8);
			}
            ENDCG
        }
    }
}
