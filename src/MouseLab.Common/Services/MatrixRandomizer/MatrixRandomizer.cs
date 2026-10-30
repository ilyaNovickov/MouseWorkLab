using MouseLab.Core.Services;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using MouseLab.Core;


namespace MouseLab.Common.Services;

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
    #region PublicInterface
    public unsafe void Randomize(Span<byte> dest, int? seed = null)
    {
        // seed ?? Random.Shared: интерфейс допускает вызов без seed —
        // тогда каждый вызов получает своё случайное состояние.
        uint baseState = (uint)(seed ?? Random.Shared.Next());

        fixed (byte* buffer = dest)
            FillRange(buffer, dest.Length, baseState);
    }

    public void Randomize(IMatrix dest, int? seed = null)
    {
        ArgumentNullException.ThrowIfNull(dest);

        Randomize(dest.SpanBytes, seed);
    }

    public IMatrix Randomize(int width, int height, int? seed = null)
    {
        var dest = new Matrix(width, height);
        Randomize(dest, seed);
        return dest;
    }

    public IMatrix Randomize(Size size, int? seed = null)
    {
        return Randomize(size.Width, size.Height, seed);
    }

    public IMatrix Randomize(int width, int height, IMatrixProvider provider, int? seed = null)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var dest = provider.Create(width, height);
        Randomize(dest, seed);
        return dest;
    }

    public IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null)
    {
        return Randomize(size.Width, size.Height, provider, seed);
    }
    #endregion PublicInterface

    #region  PrivateImpl
    // Порог параллельного разбиения по кускам буфера.
    private static readonly long ParallelCostThreshold =
        Environment.ProcessorCount >= 4 ? 4_000_000 : long.MaxValue;

    // Размер куска при параллельном заполнении (~1 млн байт).
    private const int ChunkSize = 1_048_576;

    // Шаг, разводящий состояния разных кусков и lane.
    private const uint LaneStride = 0x9E3779B9u;

    private static unsafe void FillRange(byte* pBuffer, long count, uint baseState)
    {
        if (count <= 0)
            return;

        if (count > ParallelCostThreshold)
        {
            FillParallel(pBuffer, count, baseState);
        }
        else
        {
            FillChunk(pBuffer, count, baseState);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static unsafe void FillParallel(byte* pBuffer, long count, uint baseState)
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

    // Можно убрать `AggressiveInlinig` для внутренних меодов если нужно
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void FillChunk(byte* p, long count, uint baseState)
    {
        if (Vector256.IsHardwareAccelerated)
        {
            FillVector256(p, count, baseState);
        }
        else if (Vector128.IsHardwareAccelerated)
        {
            FillVector128(p, count, baseState);
        }
        else
        {
            FillScalar(p, count, baseState);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void FillVector256(byte* p, long count, uint baseState)
    {
        var state = Vector256.Create(
            (baseState) | 1u,
            (baseState + LaneStride) | 1u,
            (baseState + unchecked(2u * LaneStride)) | 1u,
            (baseState + unchecked(3u * LaneStride)) | 1u,
            (baseState + unchecked(4u * LaneStride)) | 1u,
            (baseState + unchecked(5u * LaneStride)) | 1u,
            (baseState + unchecked(6u * LaneStride)) | 1u,
            (baseState + unchecked(7u * LaneStride)) | 1u);

        nuint vectorEnd = (nuint)count & ~((nuint)Vector256<byte>.Count - 1);

        for (nuint offset = 0; offset < vectorEnd; offset += (nuint)Vector256<byte>.Count)
        {
            state = Vector256.Xor(state, Vector256.ShiftLeft(state, 13));
            state = Vector256.Xor(state, Vector256.ShiftRightLogical(state, 17));
            state = Vector256.Xor(state, Vector256.ShiftLeft(state, 5));
            state.AsByte().Store(p + offset);
        }

        if ((long)vectorEnd < count)
            FillScalar(p + vectorEnd, count - (long)vectorEnd, baseState ^ 0x5F3A7FFDu);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void FillVector128(byte* p, long count, uint baseState)
    {
        var state = Vector128.Create(
            (baseState) | 1u,
            (baseState + LaneStride) | 1u,
            (baseState + unchecked(2u * LaneStride)) | 1u,
            (baseState + unchecked(3u * LaneStride)) | 1u);

        nuint vectorEnd = (nuint)count & ~((nuint)Vector128<byte>.Count - 1);

        for (nuint offset = 0; offset < vectorEnd; offset += (nuint)Vector128<byte>.Count)
        {
            state = Vector128.Xor(state, Vector128.ShiftLeft(state, 13));
            state = Vector128.Xor(state, Vector128.ShiftRightLogical(state, 17));
            state = Vector128.Xor(state, Vector128.ShiftLeft(state, 5));
            state.AsByte().Store(p + offset);
        }

        if ((long)vectorEnd < count)
            FillScalar(p + vectorEnd, count - (long)vectorEnd, baseState ^ 0x5F3A7FFDu);
    }

    /// <summary>Свободный от SIMD путь: та же череда xorshift, но по 4 байта.</summary>
    private static unsafe void FillScalar(byte* p, long count, uint state)
    {
        state |= 1u;

        long i = 0;

        for (; i + 4 <= count; i += 4)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            *(uint*)(p + i) = state;
        }

        for (; i < count; i++)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            p[i] = (byte)state;
        }
    }
    #endregion  PrivateImpl
}
