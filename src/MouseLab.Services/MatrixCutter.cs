using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;

namespace MouseLab.Services
{
    /// <summary>
    /// Вырезает фрагмент размером size с позиции position донора в dest:
    /// пересечение окна с донором копируется, всё, что вышло за границы
    /// донора, заполняется 0 (fillZero = true) или 255 (false).
    /// Копирование строк идёт через Span (JIT превращает в memmove),
    /// заполнение — Span.Fill; оба пути векторизованы в BCL.
    /// </summary>
    /// <remarks>
    /// Писатель перезаписывает каждый пиксель dest, поэтому арендованная
    /// из пула матрица с мусором безопасна.
    /// </remarks>
    public sealed class MatrixCutter : IMatrixCutter
    {
        // Порог параллельного разбиения по строкам dest;
        // ниже него оплата потоков превосходит выигрыш.
        private const long ParallelCostThreshold = 1_000_000;

        private readonly IMatrixProvider? _defaultProvider;

        public MatrixCutter(IMatrixProvider? defaultProvider = null)
        {
            _defaultProvider = defaultProvider;
        }

        public unsafe void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);

            if (dest.Width != size.Width || dest.Height != size.Height)
                throw new ArgumentException("Размер dest должен совпадать с size.");

            if ((long)dest.Width * dest.Height == 0)
                return;

            byte fillValue = fillZero ? (byte)0 : (byte)255;

            fixed (byte* srcFixed = src.ReadOnlySpanBytes, dstFixed = dest.SpanBytes)
            {
                // Фиксированные указатели нельзя захватить лямбдой (CS1764),
                // поэтому Parallel.For отделён в метод, принимающий указатели
                // параметрами.
                if ((long)dest.Width * dest.Height > ParallelCostThreshold)
                {
                    CutParallel(srcFixed, dstFixed, src.Width, src.Height,
                        dest.Width, dest.Height, position.X, position.Y, fillValue);
                }
                else
                {
                    for (int row = 0; row < dest.Height; row++)
                        CutRow(srcFixed, dstFixed, src.Width, src.Height, dest.Width,
                            position.X, position.Y, row, fillValue);
                }
            }
        }

        private static unsafe void CutParallel(byte* srcBase, byte* dstBase, int srcWidth, int srcHeight,
            int destWidth, int destHeight, int px, int py, byte fillValue)
        {
            Parallel.For(0, destHeight, row =>
                CutRow(srcBase, dstBase, srcWidth, srcHeight, destWidth, px, py, row, fillValue));
        }

        public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var dest = provider.Create(size.Width, size.Height);
            Cut(src, position, size, dest, fillZero);
            return dest;
        }

        public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
        {
            // Вне провайдера: обычное выделение,Dispose вернёт матрицу в пул.
            var dest = new PooledMatrix(size.Width, size.Height);
            Cut(src, position, size, dest, fillZero);
            return dest;
        }

        private static unsafe void CutRow(byte* srcBase, byte* dstBase, int srcWidth, int srcHeight,
            int destWidth, int px, int py, int row, byte fillValue)
        {
            var destRow = new Span<byte>(dstBase + (long)row * destWidth, destWidth);
            int donorY = py + row;

            if ((uint)donorY >= (uint)srcHeight)
            {
                destRow.Fill(fillValue);
                return;
            }

            // Столбцы dest, у которых донорский пиксель существует:
            // donor col = px + col ∈ [0, srcWidth).
            int from = Math.Max(0, -px);
            int to = Math.Min(srcWidth - px, destWidth);

            if (to <= from)
            {
                destRow.Fill(fillValue);
                return;
            }

            if (from > 0)
                destRow[..from].Fill(fillValue);

            var srcRow = new ReadOnlySpan<byte>(srcBase + (long)donorY * srcWidth + px + from, to - from);
            srcRow.CopyTo(destRow[from..to]);

            if (to < destWidth)
                destRow[to..].Fill(fillValue);
        }
    }
}
