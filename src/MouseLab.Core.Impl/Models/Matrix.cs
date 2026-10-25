using System;
using System.Collections.Immutable;
using MouseLab.Core.Models;

namespace MouseLab.Core.Models;

public class Matrix : IMatrix
{
    private byte[] _array;

    private Size _size;

    public Matrix(int width, int height) : this(new byte[width * height], width, height)
    {

    }

    public Matrix(byte[] array, int width, int height)
    {
        if (array.Length != width * height)
            throw new Exception("Array has to be lendth equal width * height");

        _array = array;
        _size = new Size(width, height);
    }

    Size Size => _size;

    int Width => _size.Width;

    int Height => _size.Height;

    ImmutableArray<byte> ImmutableBytes => _array.ToImmutableArray<byte>();

    byte[] Bytes => _array;

    Span<byte> SpanBytes => _array.AsSpan<byte>();

    ReadOnlySpan<byte> ReadOnlySpanBytes => _array.AsSpan<byte>();
}
