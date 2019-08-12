#ifndef VIRTUAL_TEXTURE_DEBUG_INCLUDED
#define VIRTUAL_TEXTURE_DEBUG_INCLUDED

#include "VirtualTexture.cginc"

float VTGetMipLevel(float2 uv)
{
	fixed4 page = tex2D(_VTLookupTex, uv);
	return page.b * 255;
}

fixed4 VTGetMipColor(float mip)
{
	const fixed4 colors[12] = {
		fixed4(1, 0, 0, 1),
		fixed4(0, 0, 1, 1),
		fixed4(1, 0.5f, 0, 1),
		fixed4(1, 0, 0.5f, 1),
		fixed4(0, 0.5f, 0.5f, 1),
		fixed4(0, 0.25f, 0.5f, 1),
		fixed4(0.25f, 0.5f, 0, 1),
		fixed4(0.5f, 0, 1, 1),
		fixed4(1, 0.25f, 0.5f, 1),
		fixed4(0.5f, 0.5f, 0.5f, 1),
		fixed4(0.25f, 0.25f, 0.25f, 1),
		fixed4(0.125f, 0.125f, 0.125f, 1)
	};
	return colors[clamp(mip, 0, 11)];
}


fixed4 VTDebugMipmap(sampler2D tex, float2 uv) : SV_Target
{
	return VTGetMipColor(tex2D(tex, uv).b * 255);
}


fixed4 VTFragDebug(VTV2f i) : SV_Target
{
	float2 uv = VTTransferUV(i.uv);
	fixed4 col = VTTex2D(uv) + VTGetMipColor(VTGetMipLevel(i.uv));
	return col;
}


#endif