Shader "Hidden/VirtualTexture/DebugMipmap"
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
			#include "VTDebug.cginc"	
			#include "UnityCG.cginc"
			#pragma vertex VTVert
			#pragma fragment frag

			sampler2D _MainTex;
			
			fixed4 frag(VTV2f i) : SV_Target
			{
				return VTDebugMipmap(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
