using System.Runtime.CompilerServices;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using MouseLab.Core.Services;

namespace MouseLab.Core.Impl.UnsafeVer2
{
    /// <summary>
    /// Вырезает фрагмент размером size с позиции position донора в dest:
    /// пересечение окна с донором копируется, всё, что вышло за границы
    /// донора, заполняется 0 (fillZero = true) или 255 (false).
    /// Копирование строк идёт через Span (JIT превращает в memmove),
    /// заполнение — Span.Fill; оба пути векторизованы в BCL.
    /// Однопоточная ветка не пиннит буферы: `fixed` и указатели —
    /// только в параллельной ветке. Замер показал, что Slice/CopyTo
    /// на управляемых Span'ах векторизуются так же, а пиннинг стоит
    /// дороже снятия bounds checks — отсюда расщепление веток.
    /// </summary>
    /// <remarks>
    /// Писатель перезаписывает каждый пиксель dest, поэтому арендованная
    /// из пула матрица с мусором безопасна.
    /// </remarks>
    public sealed class MatrixCutter : IMatrixCutter
    {
        #region PublicInterfaceMethods
        public void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true)
        {
            CutMatirx(src, position.X, position.Y, size.Width, size.Height, dest, fillZero);
        }

        public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, size.Width, size.Height, fillZero);
        }

        public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
        {
            return Cut(src, x, y, size.Width, size.Height, fillZero);
        }

        public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, width, height, fillZero);
        }

        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
        {
            var dest = new Matrix(width, height);
            CutMatirx(src, x, y, width, height, dest, fillZero);
            return dest;
        }

        #region  WithProvider
        public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var dest = provider.Create(size.Width, size.Height);
            Cut(src, position, size, dest, fillZero);
            return dest;
        }


        public IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, x, y, size.Width, size.Height, provider, fillZero);
        }

        public IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, width, height, provider, fillZero);
        }

        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var dest = provider.Create(width, height);
            CutMatirx(src, x, y, width, height, dest, fillZero);

            return dest;
        }
        #endregion

        #endregion

        #region  PrivateImpl
        // Порог параллельного разбиения по строкам dest;
        // ниже него оплата потоков превосходит выигрыш.
        // Замер Benchmark.MoveFinder показал, что на окнах 1-2 млн px
        // параллельная версия не даёт выигрыша против однопоточной
        // (работу поглощает ThreadPool), поэтому порог поднят
        // наравне с порогом MatrixRandomizer'а.
        private const long ParallelCostThreshold = 4_000_000;


        private static unsafe void CutMatirx(IMatrix src, int x, int y, int width, int height, IMatrix dest, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dest);

            if (dest.Width != width || dest.Height != height)
                throw new ArgumentException("Размер dest должен совпадать с size.");

            if ((long)dest.Width * dest.Height == 0)
                return;

            byte fillValue = fillZero ? (byte)0 : (byte)255;

            // Фиксированные указатели нельзя захватить лямбдой (CS1764),
            // поэтому Parallel.For отделён в метод, принимающий указатели
            // параметрами.
            if ((long)dest.Width * dest.Height > ParallelCostThreshold)
            {
                // Параллельная ветка — единственное место с пиннингом:
                // оправдан он только на объёмах, распределённых между
                // потоками.
                fixed (byte* srcFixed = src.ReadOnlySpanBytes, dstFixed = dest.SpanBytes)
                    CutParallel(srcFixed, dstFixed, src.Width, src.Height,
                        dest.Width, dest.Height, x, y, fillValue);
            }
            else
            {
                // Однопоточная ветка — управляемые Span'ы, как в старом
                // ImageCutter: без пиннинга, без указателей; Slice/CopyTo
                // векторизуются так же (см. замер Benchmark.MoveFinder).
                //
                // КЛЮЧЕВОЕ: interface-свойства src/dest — Width/Height —
                // читаются один раз перед циклом. Изоляционный замер
                // (CoreCutterBenchmark) показал, что те же самые свойства,
                // читаемые на КАЖДОЙ строке, стоят ~50-60 мкс на 384-строчном
                // окне (92-93 против 34-39 мкс) — дешёвый сам по себе
                // interface-вызов+отсутствие hoisting'а разом убивает ядро.
                ReadOnlySpan<byte> srcData = src.ReadOnlySpanBytes;
                Span<byte> dstData = dest.SpanBytes;
                int srcW = src.Width, srcH = src.Height;
                int dstW = dest.Width, dstH = dest.Height;

                int from = Math.Max(0, -x);
                int to = Math.Min(srcW - x, dstW);

                for (int row = 0; row < dstH; row++)
                {
                    var destRow = dstData.Slice(row * dstW, dstW);
                    int donorY = y + row;

                    if ((uint)donorY >= (uint)srcH)
                    {
                        destRow.Fill(fillValue);
                        continue;
                    }

                    if (to <= from)
                    {
                        destRow.Fill(fillValue);
                        continue;
                    }

                    if (from > 0)
                        destRow[..from].Fill(fillValue);

                    srcData.Slice(donorY * srcW + x + from, to - from)
                        .CopyTo(destRow[from..to]);

                    if (to < dstW)
                        destRow[to..].Fill(fillValue);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CutParallel(byte* srcBase, byte* dstBase, int srcWidth, int srcHeight,
                    int destWidth, int destHeight, int px, int py, byte fillValue)
        {
            Parallel.For(0, destHeight, row =>
                CutRow(srcBase, dstBase, srcWidth, srcHeight, destWidth, px, py, row, fillValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        #endregion






    }
}
