using System;
using System.Runtime.CompilerServices;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services.MatrixCutters.OldVersion;

/// <summary>
/// Сервис вырезания области из матриц
/// </summary>
public sealed class ImageCutter : IMatrixCutter
{
    #region PublicInterfaceMethods
    public void Cut(IMatrix src, int x, int y, int width, int height, IMatrix dest, bool fillZero = true)
    {
        if (dest.Width != width || dest.Height != height)
            throw new ArgumentException("The dest matrix has to has the same size as target matrix");
        CutInto(src, x, y, width, height, dest.SpanBytes, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Cut(IMatrix src, int x, int y, Size size, IMatrix dest, bool fillZero = true)
    {
        Cut(src, x, y, size.Width, size.Height, dest, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Cut(IMatrix src, Point position, int width, int height, IMatrix dest, bool fillZero = true)
    {
        Cut(src, position.X, position.Y, width, height, dest, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true)
    {
        Cut(src, position.X, position.Y, size.Width, size.Height, dest, fillZero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, size.Width, size.Height, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
    {
        return Cut(src, x, y, size.Width, size.Height, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, width, height, fillZero);
    }

    public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
    {
        var dest = new Matrix(width, height);
        CutInto(src, x, y, width, height, dest.SpanBytes, fillZero);
        return dest;
    }

    #region  WithProvider
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, size.Width, size.Height, provider, fillZero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true)
    {
        return Cut(src, x, y, size.Width, size.Height, provider, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, width, height, provider, fillZero);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var dest = provider.Create(width, height);
        CutInto(src, x, y, width, height, dest.SpanBytes, fillZero);

        return dest;
    }
    #endregion

    #endregion


    private static void CutInto(IMatrix src, int px, int py, int w, int h, Span<byte> dst, bool fillZero)
    {
        byte fill = fillZero ? byte.MinValue : byte.MaxValue;

        ReadOnlySpan<byte> srcData = src.ReadOnlySpanBytes;
        int sw = src.Width;
        int sh = src.Height;

        // Построчно: для каждой строки копируем непрерывный сегмент исходника
        // (внутри строки пиксели идут подряд) и заполняем граничные отступы.
        for (int y = 0; y < h; y++)
        {
            int sy = py + y;
            int dstRow = y * w;

            // Вся строка вне исходника — заполняем значением.
            if (sy < 0 || sy >= sh)
            {
                dst.Slice(dstRow, w).Fill(fill);
                continue;
            }

            int left = Math.Max(0, -px);      // столбцы с sx < 0
            int right = Math.Min(w, sw - px); // столбцы с sx >= sw

            if (left > 0)
                dst.Slice(dstRow, left).Fill(fill);

            if (right < w)
                dst.Slice(dstRow + right, w - right).Fill(fill);

            int count = right - left;
            if (count > 0)
            {
                int sx = px + left;
                srcData.Slice(sy * sw + sx, count).CopyTo(dst.Slice(dstRow + left, count));
            }
        }
    }


}
