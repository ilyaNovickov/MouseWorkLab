using System.Collections.Immutable;
using MouseLab.Core.Models;
using MouseLab.Core;

namespace MouseLab.Common.Models.PooledMatrix;

/// <summary>
/// Реализация IMatrix: непрерывный байтовый буфер «ширина × высота»,
/// один байт — пиксель в градациях серого от 0 до 255.
/// </summary>
/// <remarks>
/// Матрица, выданная пулом (через MatrixProvider), при Dispose возвращается
/// пулу, а не уничтожается. В буфере остаются старые байты, поэтому перед
/// чтением арендованную матрицу нужно полностью перезаписать; все писатели
/// (IMatrixCutter, IMatrixRandomizer) это делают автоматически.
/// Повторный Dispose безопасен. Асинхронно Dispose и чтение одной матрицы
/// из разных потоков не сочетаются.
/// </remarks>
public sealed class PooledMatrix : IMatrix
{
    private readonly byte[] _buffer;

    internal MatrixPool? Owner { get; set; }

    internal bool Returned { get; set; }

    public PooledMatrix(byte[] buffer, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ValidateDimensions(width, height);

        if (buffer.Length != width * height)
            throw new ArgumentException("Длина буфера должна быть ровно width * height.");

        _buffer = buffer;
        Width = width;
        Height = height;
    }

    public PooledMatrix(int width, int height)
    {
        ValidateDimensions(width, height);

        _buffer = new byte[width * height];
        Width = width;
        Height = height;
    }

    // Проверка отдельна от delegate-конструктора: размеры должны
    // проверяться ДО выделения буфера, иначе new byte[-4] даст
    // OverflowException вместо ожидаемого ArgumentOutOfRangeException.
    private static void ValidateDimensions(int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);
    }

    public Size Size => new(Width, Height);

    public int Width { get; }

    public int Height { get; }

    public byte[] Bytes => _buffer;

    public Span<byte> SpanBytes => _buffer.AsSpan();

    public ReadOnlySpan<byte> ReadOnlySpanBytes => _buffer;

    public ImmutableArray<byte> ImmutableBytes => ImmutableArray.Create(_buffer);

    public void Dispose()
    {
        if (Owner is not { } owner || Returned)
            return;

        Returned = true;
        owner.Return(this);
    }
}

