using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;

namespace MouseLab.Services
{
    /// <summary>
    /// Заполняет матрицу белым шумом (равномерные байты 0..255).
    ///
    /// Двигатель: xorshift32 в векторных lane (Vector256, 8 независимых
    /// состояний); на каждой итерации выдаётся 32 байта — сырые байты
    /// состояний. Детерминирован по seed: один и тот же seed даёт один и
    /// тот же буфер, в том числе при параллельном разделении (границы
    /// кусков фиксированы, у каждого куска своё состояние на основе seed).
    /// </summary>
    /// <remarks>
    /// Статистическое качество — тестовое, не криптографическое. Если
    /// гистограмма перестанет устраивать, ускоренным включением будет
    /// перемешивание байта выхода: byte = (state * константа) >> 24.
    /// Писатель перезаписывает каждый пиксель dest, поэтому арендованная
    /// из пула матрица с мусором безопасна.
    /// </remarks>
    public sealed class MatrixRandomizer : IMatrixRandomizer
    {
        // Порог параллельного разбиения по кускам буфера.
        private const long ParallelCostThreshold = 4_000_000;

        // Размер куска при параллельном заполнении (~1 млн байт).
        private const int ChunkSize = 1_048_576;

        // Шаг, разводящий состояния разных кусков и lane.
        private const uint LaneStride = 0x9E3779B9u;

        public unsafe void Randomize(IMatrix dest, int? seed = null)
        {
            ArgumentNullException.ThrowIfNull(dest);

            // seed ?? Random.Shared: интерфейс допускает вызов без seed —
            // тогда каждый вызов получает своё случайное состояние.
            uint baseState = (uint)(seed ?? Random.Shared.Next());

            fixed (byte* buffer = dest.SpanBytes)
                FillRange(buffer, (long)dest.Width * dest.Height, baseState);
        }

        public IMatrix Randomize(int width, int height, int? seed = null)
        {
            var dest = new PooledMatrix(width, height);
            Randomize(dest, seed);
            return dest;
        }

        public IMatrix Randomize(int width, int height, IMatrixProvider provider, int? seed = null)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var dest = provider.Create(width, height);
            Randomize(dest, seed);
            return dest;
        }

        private static unsafe void FillRange(byte* pBuffer, long count, uint baseState)
        {
            if (count <= 0)
                return;

            if (count > ParallelCostThreshold)
            {
                long chunks = (count + ChunkSize - 1) / ChunkSize;

                Parallel.For(0, (int)chunks, chunk =>
                {
                    long start = (long)chunk * ChunkSize;
                    long length = Math.Min(ChunkSize, count - start);
                    uint chunkState = baseState ^ (uint)chunk * LaneStride ^ 0x85EBCA6Bu;

                    FillChunk(pBuffer + start, length, chunkState);
                });
            }
            else
            {
                FillChunk(pBuffer, count, baseState);
            }
        }

        private static unsafe void FillChunk(byte* p, long count, uint baseState)
        {
            if (!Vector256.IsHardwareAccelerated)
            {
                FillScalar(p, count, baseState);
                return;
            }

            var state = Vector256.Create(
                baseState,
                baseState + LaneStride,
                baseState + unchecked(2u * LaneStride),
                baseState + unchecked(3u * LaneStride),
                baseState + unchecked(4u * LaneStride),
                baseState + unchecked(5u * LaneStride),
                baseState + unchecked(6u * LaneStride),
                baseState + unchecked(7u * LaneStride));

            nuint vectorEnd = (nuint)count & ~((nuint)Vector256<byte>.Count - 1);

            for (nuint offset = 0; offset < vectorEnd; offset += (nuint)Vector256<byte>.Count)
            {
                state = Vector256.Xor(state, Vector256.ShiftLeft(state, 13));
                state = Vector256.Xor(state, Vector256.ShiftRightLogical(state, 17));
                state = Vector256.Xor(state, Vector256.ShiftLeft(state, 5));

                var bytes = Unsafe.As<Vector256<uint>, Vector256<byte>>(ref state);
                Vector256.Store(bytes, p + offset);
            }

            if ((long)vectorEnd < count)
                FillScalar(p + vectorEnd, count - (long)vectorEnd, baseState ^ 0x5F3A7FFDu);
        }

        /// <summary>Свободный от SIMD путь: та же череда xorshift, но по 4 байта.</summary>
        private static unsafe void FillScalar(byte* p, long count, uint state)
        {
            long i = 0;

            for (; i + 4 <= count; i += 4)
            {
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;

                p[i] = (byte)state;
                p[i + 1] = (byte)(state >> 8);
                p[i + 2] = (byte)(state >> 16);
                p[i + 3] = (byte)(state >> 24);
            }

            for (; i < count; i++)
            {
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                p[i] = (byte)state;
            }
        }
    }
}
