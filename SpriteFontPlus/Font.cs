using System;
using System.Runtime.InteropServices;

namespace SpriteFontPlus;

sealed class Font : IDisposable {
    readonly Int32Map<int> _kernings = new();

    float _ascentBase, _descentBase, _lineHeightBase;

    IntPtr _font;

    public float Ascent { get; private set; }
    public float Descent { get; private set; }
    public float LineHeight { get; private set; }
    public float Scale { get; private set; }

    public void Dispose() {
        if (_font != IntPtr.Zero) {
            NativeMethods.FontInfoRelease(_font);
            _font = IntPtr.Zero;
        }
    }

    public void Recalculate(float size) {
        Ascent = _ascentBase * size;
        Descent = _descentBase * size;
        LineHeight = _lineHeightBase * size;
        Scale = NativeMethods.ScaleForPixelHeight(_font, size);
    }

    public int GetGlyphIndex(int codepoint) {
        return NativeMethods.FindGlyphIndex(_font, codepoint);
    }

    public unsafe void BuildGlyphBitmap(int glyph, float scale, int* advance, int* lsb, int* x0, int* y0, int* x1, int* y1) {
        NativeMethods.GetGlyphHMetrics(_font, glyph, advance, lsb);
        NativeMethods.GetGlyphBitmapBox(_font, glyph, scale, scale, x0, y0, x1, y1);
    }

    public unsafe void RenderGlyphBitmap(byte* output, int outWidth, int outHeight, int outStride, int glyph) {
        NativeMethods.MakeGlyphBitmap(_font, output, outWidth, outHeight, outStride, Scale, Scale, glyph);
    }

    public int GetGlyphKernAdvance(int glyph1, int glyph2) {
        var key = ((glyph1 << 16) | (glyph1 >> 16)) ^ glyph2;
        if (_kernings.TryGetValue(key, out var result)) {
            return result;
        }
        result = NativeMethods.GetGlyphKernAdvance(_font, glyph1, glyph2);
        _kernings[key] = result;
        return result;
    }

    public static unsafe Font FromMemory(ReadOnlySpan<byte> data) {
        var font = new Font();
        fixed (byte* p = data) {
            font._font = NativeMethods.FontInfoAlloc(p, data.Length);
        }

        if (NativeMethods.InitFont(font._font, 0) == 0) {
            throw new("stbtt_InitFont failed");
        }

        int ascent, descent, lineGap;
        NativeMethods.GetFontVMetrics(font._font, &ascent, &descent, &lineGap);

        var fh = ascent - descent;
        font._ascentBase = ascent / (float)fh;
        font._descentBase = descent / (float)fh;
        font._lineHeightBase = (fh + lineGap) / (float)fh;

        return font;
    }

    static class NativeMethods {
#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "FontInfoAlloc", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern unsafe IntPtr FontInfoAlloc(void* data, int dataLength);
#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "FontInfoRelease", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern void FontInfoRelease(IntPtr font);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "InitFont", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern int InitFont(IntPtr font, int offset);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "GetFontVMetrics", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern unsafe void GetFontVMetrics(IntPtr font, int* ascent, int* descent, int* linegap);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "MakeGlyphBitmap", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern unsafe void MakeGlyphBitmap(IntPtr font, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "GetGlyphBitmapBox", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern unsafe void GetGlyphBitmapBox(IntPtr font, int glyph, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "GetGlyphHMetrics", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern unsafe void GetGlyphHMetrics(IntPtr font, int glyph_index, int* advanceWidth, int* leftSideBearing);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "FindGlyphIndex", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern int FindGlyphIndex(IntPtr font, int unicode_codepoint);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "ScaleForPixelHeight", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern float ScaleForPixelHeight(IntPtr font, float pixels);

#if !CONSOLE
        [DllImport("SpriteFontPlus.Native", EntryPoint = "GetGlyphKernAdvance", CallingConvention = CallingConvention.Cdecl)]
#endif
        internal static extern int GetGlyphKernAdvance(IntPtr font, int glyph1, int glyph2);
    }
}
