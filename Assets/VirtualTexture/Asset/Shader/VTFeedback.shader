Shader "Hidden/VirtualTexture/Feedback"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
	{
		Tags{ "VirtualTextureType" = "Normal" }

		Pass
		{
			CGPROGRAM
			#include "VTFeedback.cginc"	
			#pragma vertex VTVert
			#pragma fragment VTFragFeedback
			ENDCG
		}
	}
}
