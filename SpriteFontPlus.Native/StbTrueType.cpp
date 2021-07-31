
#include "./Shared.hpp"
#define STB_TRUETYPE_IMPLEMENTATION
#define STBTT_STATIC
#pragma warning(push)
#pragma warning(disable: 4100) //Unreferenced formal parameter
#pragma warning(disable: 4244) //Conversion, possible loss of data
#pragma warning(disable: 4512) //Assignment operator could not be generated
#pragma warning(disable: 4127) //Conditional expression is constant
#pragma warning(disable: 4505) 
#include "./stb_truetype.h"
#pragma warning(pop)

BW_EXTERN_C

BW_DECLSPEC void* BW_CDECL FontInfoAlloc(void* data, int dataLength) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)malloc(sizeof(stbtt_fontinfo));
	info->data = (unsigned char*)malloc(dataLength);
	memcpy(info->data, data, dataLength);
	return info;
}

BW_DECLSPEC void BW_CDECL FontInfoRelease(void* font) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	free(info->data);
	free(info);
}

BW_DECLSPEC int BW_CDECL InitFont(void* font, int offset) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	unsigned char* data = info->data;
	return stbtt_InitFont(info, data, offset);
}

BW_DECLSPEC void BW_CDECL GetFontVMetrics(void* font, int* ascent, int* descent, int* linegap) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	stbtt_GetFontVMetrics(info, ascent, descent, linegap);
}

BW_DECLSPEC void BW_CDECL MakeGlyphBitmap(void* font, void* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	stbtt_MakeGlyphBitmap(info, (unsigned char*)output, out_w, out_h, out_stride, scale_x, scale_y, glyph);
}

BW_DECLSPEC void BW_CDECL GetGlyphBitmapBox(void* font, int glyph, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	stbtt_GetGlyphBitmapBox(info, glyph, scale_x, scale_y, ix0, iy0, ix1, iy1);
}

BW_DECLSPEC void BW_CDECL GetGlyphHMetrics(void* font, int glyph_index, int* advanceWidth, int* leftSideBearing) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	stbtt_GetGlyphHMetrics(info, glyph_index, advanceWidth, leftSideBearing);
}

BW_DECLSPEC int BW_CDECL FindGlyphIndex(void* font, int unicode_codepoint) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	return stbtt_FindGlyphIndex(info, unicode_codepoint);
}

BW_DECLSPEC float BW_CDECL ScaleForPixelHeight(void* font, float pixels) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	return stbtt_ScaleForPixelHeight(info, pixels);
}

BW_DECLSPEC int BW_CDECL GetGlyphKernAdvance(void* font, int glyph1, int glyph2) {
	stbtt_fontinfo* info = (stbtt_fontinfo*)font;
	return stbtt_GetGlyphKernAdvance(info, glyph1, glyph2);
}

BW_EXTERN_C_END

