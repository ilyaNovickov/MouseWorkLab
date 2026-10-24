using System.Collections.Immutable;
using MouseLab.Core;
using MouseLab.Core.Models;

namespace MouseLab.Matcher.Tests
{
    /// <summary>
    /// Тестовая реализация IMatrix: копия байтов фрагмента, вырезанного
    /// из «донора». Содержимое хранит целиком, поэтому фрагменты, вырезанные
    /// с разными смещениями, честно перекрываются только там, где это
    /// подтверждается общими байтами донора.
    /// </summary>
    internal sealed class TestMatrix : IMatrix
    {
        private readonly byte[] _data;

        public TestMatrix(byte[] data, int width, int height)
        {
            _data = data;
            Width = width;
            Height = height;
        }

        public Size Size => new Size(Width, Height);

        public int Width { get; }

        public int Height { get; }

        public ImmutableArray<byte> ImmutableBytes => ImmutableArray.Create(_data);

        public byte[] Bytes => _data;

        public Span<byte> SpanBytes => _data.AsSpan();

        public ReadOnlySpan<byte> ReadOnlySpanBytes => _data;

        public void Dispose() { }
    }
}
