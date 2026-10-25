using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using MouseLab.Core.Impl.UnsafeVer2;
using Xunit;

namespace MouseLab.Services.Tests
{
    public class MatrixCutterTests
    {
        private readonly MatrixCutter _cutter = new();

        /// <summary>Донор 8×8 со значениями 0..63.</summary>
        private static TestMatrix Donor()
        {
            var data = new byte[64];

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            return new TestMatrix(data, 8, 8);
        }

        [Fact]
        public void InteriorCutCopiesExactly()
        {
            var src = Donor();
            using var dest = new PooledMatrix(4, 3);

            _cutter.Cut(src, new Point(2, 1), new Size(4, 3), dest);

            Assert.Equal(new byte[] { 10, 11, 12, 13 }, dest.Bytes[0..4]);
            Assert.Equal(new byte[] { 18, 19, 20, 21 }, dest.Bytes[4..8]);
            Assert.Equal(new byte[] { 26, 27, 28, 29 }, dest.Bytes[8..12]);
        }

        [Fact]
        public void LeftOverflowFillsOnlyOutOfBounds()
        {
            var src = Donor();
            using var dest = new PooledMatrix(5, 2);

            _cutter.Cut(src, new Point(-2, 3), new Size(5, 2), dest, fillZero: true);

            // Строка 3 донора: первые два столбца dest — за границей.
            Assert.Equal(new byte[] { 0, 0, 24, 25, 26 }, dest.Bytes[0..5]);
            Assert.Equal(new byte[] { 0, 0, 32, 33, 34 }, dest.Bytes[5..10]);
        }

        [Fact]
        public void RightOverflowFillsTail()
        {
            var src = Donor();
            using var dest = new PooledMatrix(4, 1);

            _cutter.Cut(src, new Point(6, 0), new Size(4, 1), dest, fillZero: true);

            Assert.Equal(new byte[] { 6, 7, 0, 0 }, dest.Bytes);
        }

        [Fact]
        public void RowsBeyondDonorFillFully()
        {
            var src = Donor();
            using var dest = new PooledMatrix(3, 3);

            _cutter.Cut(src, new Point(0, -1), new Size(3, 3), dest, fillZero: true);

            Assert.Equal(new byte[] { 0, 0, 0 }, dest.Bytes[0..3]);
            Assert.Equal(new byte[] { 0, 1, 2 }, dest.Bytes[3..6]);
            Assert.Equal(new byte[] { 8, 9, 10 }, dest.Bytes[6..9]);
        }

        [Fact]
        public void FullyOutsideCutFillsEverything()
        {
            var src = Donor();
            using var dest = new PooledMatrix(4, 4);

            _cutter.Cut(src, new Point(100, 100), new Size(4, 4), dest, fillZero: false);

            Assert.All(dest.Bytes, b => Assert.Equal(255, b));
        }

        [Fact]
        public void FillZeroFalseMeansWhite()
        {
            var src = Donor();
            using var dest = new PooledMatrix(5, 2);

            _cutter.Cut(src, new Point(7, 0), new Size(5, 2), dest, fillZero: false);

            Assert.Equal(new byte[] { 7, 255, 255, 255, 255 }, dest.Bytes[0..5]);
        }

        [Fact]
        public void ProviderPathMatchesManagedPath()
        {
            var src = Donor();
            var provider = new MatrixProvider();

            using var viaProvider = _cutter.Cut(src, new Point(1, 2), new Size(6, 4), provider, fillZero: false);
            using var managed = _cutter.Cut(src, new Point(1, 2), new Size(6, 4), fillZero: false);

            Assert.Equal(managed.Bytes, viaProvider.Bytes);
        }

        [Fact]
        public void MismatchedDestSizeThrows()
        {
            var src = Donor();

            Assert.Throws<ArgumentException>(() =>
                _cutter.Cut(src, new Point(0, 0), new Size(4, 4), new PooledMatrix(4, 5)));
        }
    }

    /// <summary>Минимальная IMatrix-обёртка над массивом для тестов.</summary>
    internal sealed class TestMatrix : IMatrix
    {
        private readonly byte[] _data;

        public TestMatrix(byte[] data, int width, int height)
        {
            _data = data;
            Width = width;
            Height = height;
        }

        public Size Size => new(Width, Height);

        public int Width { get; }

        public int Height { get; }

        public byte[] Bytes => _data;

        public Span<byte> SpanBytes => _data.AsSpan();

        public ReadOnlySpan<byte> ReadOnlySpanBytes => _data;

        public System.Collections.Immutable.ImmutableArray<byte> ImmutableBytes =>
            System.Collections.Immutable.ImmutableArray.Create(_data);

        public void Dispose() { }
    }
}
