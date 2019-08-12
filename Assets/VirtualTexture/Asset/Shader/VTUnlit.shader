Shader "VirtualTexture/Unlit"
{
    SubShader
    {
		Tags{ "VirtualTextureType" = "Normal" }
        LOD 100

        Pass
        {
            CGPROGRAM
			#include "VTShading.cginc"	
            #pragma vertex VTVert
            #pragma fragment VTFragUnlit
            ENDCG
        }
    }
}
