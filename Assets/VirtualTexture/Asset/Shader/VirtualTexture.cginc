#ifndef VIRTUAL_TEXTURE_INCLUDED
#define VIRTUAL_TEXTURE_INCLUDED

struct VTAppdata {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
};

struct VTV2f
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

// x: page size
// y: vertual texture size
// z: max mipmap level
// w: mipmap level bias
float4 _VTFeedbackParam;

// xy: page count
// z:  max mipmap level
float4 _VTPageParam;

// x: padding size
// y: center size
// zw: 1 / tile count
float4 _VTTileParam;

sampler2D _VTLookupTex;

sampler2D _VTTiledTex0;
sampler2D _VTTiledTex1;
sampler2D _VTTiledTex2;
sampler2D _VTTiledTex3;

VTV2f VTVert(VTAppdata v)
{
	VTV2f o;
	UNITY_INITIALIZE_OUTPUT(VTV2f, o);

	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = v.texcoord;
	return o;
}

float2 VTTransferUV(float2 uv)
{
	float2 uvInt = uv - frac(uv * _VTPageParam.x) * _VTPageParam.y;
	fixed4 page = tex2D(_VTLookupTex, uvInt) * 255;
	float2 inPageOffset = frac(uv * exp2(_VTPageParam.z - page.b));
	float2 inTileOffset = inPageOffset * _VTTileParam.y + _VTTileParam.x;
	return (page.rg + inTileOffset) * _VTTileParam.zw;
}


fixed4 VTTex2D0(float2 uv)
{
	return tex2D(_VTTiledTex0, uv);
}

fixed4 VTTex2D1(float2 uv)
{
	return tex2D(_VTTiledTex1, uv);
}

fixed4 VTTex2D2(float2 uv)
{
	return tex2D(_VTTiledTex2, uv);
}

fixed4 VTTex2D3(float2 uv)
{
	return tex2D(_VTTiledTex3, uv);
}

fixed4 VTTex2D(float2 uv)
{
	return VTTex2D0(uv);
}

#endif