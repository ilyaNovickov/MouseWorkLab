using MouseLab.Common.Models.PooledMatrix;
using MouseLab.Common.Providers;
using Xunit;

namespace MouseLab.Core.Tests
{
    public class PooledMatrixTests
    {
        [Fact]
        public void WritesAreVisibleThroughAllViews()
        {
            using var matrix = new PooledMatrix(4, 3);

            matrix.SpanBytes.Fill(0x5A);

            Assert.Equal(0x5A, matrix.Bytes[0]);
            Assert.Equal(0x5A, matrix.ReadOnlySpanBytes[^1]);
            Assert.Equal(0x5A, matrix.ImmutableBytes[^1]);
            Assert.Equal(-1, matrix.SpanBytes.IndexOf((byte)0x00));
        }

        [Fact]
        public void NegativeDimensionsThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PooledMatrix(-1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PooledMatrix(4, -1));
        }

        [Fact]
        public void WrongBufferLengthThrows()
        {
            Assert.Throws<ArgumentException>(() => new PooledMatrix(new byte[10], 3, 3));
        }

        [Fact]
        public void UnpooledDisposeIsNoop()
        {
            var matrix = new PooledMatrix(3, 3);
            matrix.Dispose();
            matrix.Dispose();
        }

        [Fact]
        public void ReturnedBufferKeepsItsData()
        {
            // Контракт пула: арендованный буфер содержит мусор от прошлого
            // владельца; писатели обязаны полностью перезаписать его.
            var provider = new PooledMatrixProvider();
            var matrix = (PooledMatrix)provider.Create(10, 10);
            matrix.SpanBytes.Fill(0xAA);
            matrix.Dispose();

            var next = (PooledMatrix)provider.Create(10, 10);

            Assert.Equal(0xAA, next.Bytes[0]);

            next.Dispose();
        }
    }

    public class MatrixProviderTests
    {
        [Fact]
        public void CreateReturnsRightDimensions()
        {
            using var matrix = (PooledMatrix)new PooledMatrixProvider().Create(11, 5);

            Assert.Equal(11, matrix.Width);
            Assert.Equal(5, matrix.Height);
            Assert.Equal(11 * 5, matrix.Bytes.Length);
        }

        [Fact]
        public void PoolReusesSameInstance()
        {
            var provider = new PooledMatrixProvider();

            var first = (PooledMatrix)provider.Create(10, 10);
            first.Dispose();
            var second = (PooledMatrix)provider.Create(10, 10);

            Assert.Same(first, second);
        }

        [Fact]
        public void DifferentDimensionsDoNotReuse()
        {
            var provider = new PooledMatrixProvider();

            var first = (PooledMatrix)provider.Create(10, 10);
            first.Dispose();
            var second = (PooledMatrix)provider.Create(12, 10);

            Assert.NotSame(first, second);
        }

        [Fact]
        public void SeparateProvidersDoNotSharePool()
        {
            var one = new PooledMatrixProvider();
            var two = new PooledMatrixProvider();

            var first = (PooledMatrix)one.Create(8, 8);
            first.Dispose();
            var second = (PooledMatrix)two.Create(8, 8);

            Assert.NotSame(first, second);
        }
    }
}
