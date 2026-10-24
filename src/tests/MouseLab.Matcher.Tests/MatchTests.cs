using MouseLab.Core;
using MouseLab.Services;
using Xunit;

namespace MouseLab.Matcher.Tests
{
    public class MatchTests
    {
        private readonly SimdSadMatrixMatcher _matcher = new();

        [Fact]
        public void PositiveVectorIsRecovered()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 1);

            var a = TestImageFactory.Cut(donor, 64, 0, 0, 10, 10);
            var b = TestImageFactory.Cut(donor, 64, 3, 2, 10, 10);

            Assert.Equal(new Vector(3, 2), _matcher.Match(a, b, 5, 5));
        }

        [Fact]
        public void NegativeVectorIsRecovered()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 2);

            var a = TestImageFactory.Cut(donor, 64, 5, 4, 10, 10);
            var b = TestImageFactory.Cut(donor, 64, 0, 0, 10, 10);

            Assert.Equal(new Vector(-5, -4), _matcher.Match(a, b, 6, 5));
        }

        [Fact]
        public void ZeroVectorIsRecovered()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 3);

            var a = TestImageFactory.Cut(donor, 64, 7, 9, 10, 10);
            var b = TestImageFactory.Cut(donor, 64, 7, 9, 10, 10);

            Assert.Equal(Vector.Zero, _matcher.Match(a, b, 4, 4));
        }

        [Fact]
        public void OneColumnOverlapIsEnough()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 4);

            var a = TestImageFactory.Cut(donor, 64, 0, 0, 10, 10);
            var b = TestImageFactory.Cut(donor, 64, 9, 0, 10, 10);

            Assert.Equal(new Vector(9, 0), _matcher.Match(a, b, 9, 0));
        }

        [Fact]
        public void NonVectorizedTailIsCorrect()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 5);

            var a = TestImageFactory.Cut(donor, 64, 0, 0, 33, 10);
            var b = TestImageFactory.Cut(donor, 64, 2, 1, 33, 10);

            Assert.Equal(new Vector(2, 1), _matcher.Match(a, b, 3, 3));
        }

        [Fact]
        public void ResultIsDeterministic()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 6);

            var a = TestImageFactory.Cut(donor, 64, 1, 1, 16, 16);
            var b = TestImageFactory.Cut(donor, 64, 4, 6, 16, 16);

            var first = _matcher.Match(a, b, 8, 8);
            var second = _matcher.Match(a, b, 8, 8);

            Assert.Equal(first, second);
        }

        [Fact]
        public void ParallelPathFindsVector()
        {
            // 33 x 33, окно ±16: (33*33)*(33*33) ~ 1,2 млн — ещё перебор;
            // 128 x 128, окно ±32: (65*65)*(128*128) ~ 69 млн — параллельный путь.
            var donor = TestImageFactory.Noise(512, 512, seed: 7);

            var a = TestImageFactory.Cut(donor, 512, 0, 0, 128, 128);
            var b = TestImageFactory.Cut(donor, 512, 11, 17, 128, 128);

            Assert.Equal(new Vector(11, 17), _matcher.Match(a, b, 32, 32));
        }

        [Fact]
        public void VectorsAreRecoverableFromLargeDonor()
        {
            var donor = TestImageFactory.Noise(500, 500, seed: 8);

            var a = TestImageFactory.Cut(donor, 500, 20, 20, 128, 128);
            var b = TestImageFactory.Cut(donor, 500, 84, 13, 128, 128);

            Assert.Equal(new Vector(64, -7), _matcher.Match(a, b, 64, 20));
        }

        [Theory]
        [InlineData(-1, 0, false)]
        [InlineData(0, -1, false)]
        [InlineData(10, 0, false)] // maxX == ширина: перекрытия нет ни при одном кандидате
        [InlineData(0, 10, false)] // maxY == высота
        [InlineData(0, 0, true)]   // размеры не совпадают
        public void InvalidArgumentsThrow(int maxX, int maxY, bool misalignedSizes)
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 9);

            var a = TestImageFactory.Cut(donor, 64, 0, 0, misalignedSizes ? 8 : 10, 10);
            var b = TestImageFactory.Cut(donor, 64, 0, 0, 10, 10);

            Assert.ThrowsAny<ArgumentException>(() => _matcher.Match(a, b, maxX, maxY));
        }

        [Fact]
        public void NullMatrixThrows()
        {
            var donor = TestImageFactory.Noise(64, 64, seed: 10);
            var a = TestImageFactory.Cut(donor, 64, 0, 0, 10, 10);

            Assert.ThrowsAny<ArgumentNullException>(() => _matcher.Match(null!, a, 1, 1));
            Assert.ThrowsAny<ArgumentNullException>(() => _matcher.Match(a, null!, 1, 1));
        }
    }
}
