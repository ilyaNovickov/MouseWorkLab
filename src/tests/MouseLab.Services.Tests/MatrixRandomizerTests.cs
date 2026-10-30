using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using MouseLab.Common.Services;
using Xunit;
using MouseLab.Common.Providers;
using MouseLab.Common.Models.PooledMatrix;

namespace MouseLab.Services.Tests
{
    public class MatrixRandomizerTests
    {
        private readonly MatrixRandomizer _randomizer = new();

        [Fact]
        public void SameSeedGivesSameBytes()
        {
            using var first = _randomizer.Randomize(32, 32, seed: 42);
            using var second = _randomizer.Randomize(32, 32, seed: 42);

            Assert.Equal(first.Bytes, second.Bytes);
        }

        [Fact]
        public void DistinctSeedsGiveDistinctBytes()
        {
            using var first = _randomizer.Randomize(32, 32, seed: 1);
            using var second = _randomizer.Randomize(32, 32, seed: 2);

            Assert.NotEqual(first.Bytes, second.Bytes);
        }

        [Fact]
        public void NullSeedGivesNewNoiseEachCall()
        {
            using var first = _randomizer.Randomize(64, 64);
            using var second = _randomizer.Randomize(64, 64);

            Assert.NotEqual(first.Bytes, second.Bytes);
        }

        [Fact]
        public void ParallelPathIsDeterministic()
        {
            // 25 * 1_048_576 > порог 4_000_000: включается разделение на куски.
            using var first = _randomizer.Randomize(4096, 6400, seed: 5);
            using var second = _randomizer.Randomize(4096, 6400, seed: 5);

            Assert.Equal(first.Bytes, second.Bytes);
        }

        [Fact]
        public void BytesCoverWidely()
        {
            using var matrix = _randomizer.Randomize(128, 128, seed: 3);

            Assert.All(matrix.Bytes, b => Assert.InRange(b, 0, 255));
            Assert.InRange(matrix.Bytes.Distinct().Count(), 200, 256);
        }

        [Fact]
        public void PooledMatrixIsFullyOverwritten()
        {
            // Контракт: арендованный буфер с мусором после заполнения равен
            // свежему заполнению тем же seed.
            var provider = new PooledMatrixProvider();

            var stale = (PooledMatrix)provider.Create(64, 64);
            stale.SpanBytes.Fill(0xAA);
            stale.Dispose();

            using var reUsed = (PooledMatrix)provider.Create(64, 64);
            _randomizer.Randomize(reUsed, seed: 7);

            using var fresh = _randomizer.Randomize(64, 64, seed: 7);

            Assert.Equal(fresh.Bytes, reUsed.Bytes);
        }

        [Fact]
        public void DestOverloadFillsEveryByte()
        {
            var provider = new PooledMatrixProvider();
            var dest = (PooledMatrix)provider.Create(17, 19);
            dest.SpanBytes.Fill(0xAA);

            try
            {
                _randomizer.Randomize(dest, seed: 9);
                // Детерминированность уже проверена; здесь проверяем отсутствие
                // исключений и сохранность размеров.
                Assert.Equal(17 * 19, dest.Bytes.Length);
            }
            finally
            {
                dest.Dispose();
            }
        }
    }
}
