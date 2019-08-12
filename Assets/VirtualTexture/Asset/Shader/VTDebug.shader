Shader "VirtualTexture/Debug"
{
    SubShader
    {
		Tags{ "VirtualTextureType" = "Normal" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#include "VTDebug.cginc"	
            #pragma vertex VTVert
            #pragma fragment VTFragDebug
            ENDCG
        }
    }
}
