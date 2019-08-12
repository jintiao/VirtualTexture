#ifndef VIRTUAL_TEXTURE_SHADING_INCLUDED
#define VIRTUAL_TEXTURE_SHADING_INCLUDED

#include "VirtualTexture.cginc"

fixed4 VTFragUnlit(VTV2f i) : SV_Target
{
	float2 uv = VTTransferUV(i.uv);
	fixed4 col = VTTex2D(uv);
	return col;
}

#endif