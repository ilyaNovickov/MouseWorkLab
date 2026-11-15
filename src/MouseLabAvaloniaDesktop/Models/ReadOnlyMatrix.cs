using System;
using System.Collections.Immutable;

namespace MouseLabAvaloniaDesktop.Models;

public class ReadOnlyMatrix : IReadOnlyMatrix
{
    private byte[] _array;

    private Size _size;

    public ReadOnlyMatrix(int width, int height) : this(new byte[width * height], width, height)
    {

    }

    public ReadOnlyMatrix(byte[] array, int width, int height)
    {
        if (width < 0 || height < 0)
            throw new ArgumentException("Dimension must be positive");
        if (array.Length != width * height)
            throw new Exception("Array has to be length equal width * height");

        _array = array;
        _size = new Size(width, height);
    }

    public Size Size => new Size(_size.Width, _size.Height);

    public int Width => _size.Width;

    public int Height => _size.Height;

    public ImmutableArray<byte> ImmutableBytes => _array.ToImmutableArray<byte>();

    public ReadOnlySpan<byte> ReadOnlySpanBytes => _array.AsSpan<byte>();

    public void Dispose()
    {

    }
}
