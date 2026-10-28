using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public void Cut(IMatrix src, int x, int y, int width, int height, IMatrix dest, bool fillZero = true)
        {
            if (dest.Width != width || dest.Height != height)
                throw new ArgumentException("The dest matrix has to has the same size as target matrix");
            CutMatrix(src.ReadOnlySpanBytes, src.Width, src.Height,
                x, y, width, height, dest.SpanBytes, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cut(IMatrix src, int x, int y, Size size, IMatrix dest, bool fillZero = true)
        {
            Cut(src, x, y, size.Width, size.Height, dest, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cut(IMatrix src, Point position, int width, int height, IMatrix dest, bool fillZero = true)
        {
            Cut(src, position.X, position.Y, width, height, dest, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true)
        {
            Cut(src, position.X, position.Y, size.Width, size.Height, dest, fillZero);
        }

        #region WithNewMatrix
        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
        {
            var dest = new Matrix(width, height);
            CutMatrix(src.ReadOnlySpanBytes, src.Width, src.Height, x, y, width, height, dest.SpanBytes, fillZero);
            return dest;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, size.Width, size.Height, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
        {
            return Cut(src, x, y, size.Width, size.Height, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, width, height, fillZero);
        }

        #endregion WithNewMatrix

        #region  WithProvider
        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            ArgumentNullException.ThrowIfNull(provider);

            var dest = provider.Create(width, height);
            CutMatrix(src.ReadOnlySpanBytes, src.Width, src.Height, x, y, width, height, dest.SpanBytes, fillZero);

            return dest;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, size.Width, size.Height, provider, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, x, y, size.Width, size.Height, provider, fillZero);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, position.X, position.Y, width, height, provider, fillZero);
        }

        #endregion WithProvider

        #endregion

        #region  PrivateImpl
        // Порог параллельного разбиения по строкам dest;
        // ниже него оплата потоков превосходит выигрыш.
        // Замер Benchmark.MoveFinder показал, что на окнах 1-2 млн px
        // параллельная версия не даёт выигрыша против однопоточной
        // (работу поглощает ThreadPool), поэтому порог поднят
        // наравне с порогом MatrixRandomizer'а.
        private static readonly long ParallelCostThreshold =
            Environment.ProcessorCount >= 4
            ? 4_000_000    // 4+ ядер: параллелизм оправдан
            : long.MaxValue; // 2 ядра: отключён

        //можно заменить IMatrix dest на Span
        private static unsafe void CutMatrix(ReadOnlySpan<byte> src, int srcWidth, int srcHeight,
            int x, int y, int width, int height, Span<byte> dest, bool fillZero = true)
        {
            if ((long)width * height == 0)
                return;

            byte fillValue = fillZero ? (byte)0 : (byte)255;

            // Фиксированные указатели нельзя захватить лямбдой (CS1764),
            // поэтому Parallel.For отделён в метод, принимающий указатели
            // параметрами.
            if ((long)width * height > ParallelCostThreshold)
            {
                // Параллельная ветка — единственное место с пиннингом:
                // оправдан он только на объёмах, распределённых между
                // потоками.
                fixed (byte* srcFixed = src, dstFixed = dest)
                    CutParallel(srcFixed, dstFixed, srcWidth, srcHeight,
                        width, height, x, y, fillValue);
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
                int dstW = width, dstH = height;

                // Fast path: entire cut region is inside source
                if (x >= 0 && y >= 0 && dstW <= srcWidth - x && dstH <= srcHeight - y)
                {
                    ref byte srcBase = ref MemoryMarshal.GetReference(src);
                    ref byte dstBase = ref MemoryMarshal.GetReference(dest);

                    for (int row = 0; row < dstH; row++)
                    {
                        /*
                        src.Slice((y + row) * srcWidth + x, dstW)
                           .CopyTo(dest.Slice(row * dstW, dstW));
                        */
                        Unsafe.CopyBlockUnaligned(
                            ref Unsafe.Add(ref dstBase, (nint)row * dstW),
                            ref Unsafe.Add(ref srcBase, ((nint)y + row) * srcWidth + x),
                            (uint)dstW);
                    }
                    return;
                }

                int from = Math.Max(0, -x);
                int to = Math.Min(srcWidth - x, dstW);

                if (to <= from)
                {
                    // Вся dest-область вне source по X — заполнить целиком
                    dest.Slice(0, dstW * dstH).Fill(fillValue);
                    return;
                }

                for (int row = 0; row < dstH; row++)
                {
                    var destRow = dest.Slice(row * dstW, dstW);
                    int donorY = y + row;

                    if ((uint)donorY >= (uint)srcHeight)
                    {
                        destRow.Fill(fillValue);
                        continue;
                    }

                    if (from > 0)
                        destRow[..from].Fill(fillValue);

                    src.Slice(donorY * srcWidth + x + from, to - from)
                        .CopyTo(destRow[from..to]);

                    if (to < dstW)
                        destRow[to..].Fill(fillValue);
                }
            }
        }

        private static unsafe void CutParallel(byte* srcBase, byte* dstBase,
            int srcWidth, int srcHeight,
            int destWidth, int destHeight,
            int px, int py, byte fillValue)
        {
            // Fast path: entire region inside source
            if (px >= 0 && py >= 0 &&
                destWidth <= srcWidth - px &&
                destHeight <= srcHeight - py)
            {
                Parallel.For(0, destHeight, row =>
                {
                    Unsafe.CopyBlockUnaligned(
                        ref *(dstBase + (nint)row * destWidth),
                        ref *(srcBase + ((nint)py + row) * srcWidth + px),
                        (uint)destWidth);
                });
                return;
            }

            // Slow path: boundary handling
            int from = Math.Max(0, -px);
            int to = Math.Min(srcWidth - px, destWidth);

            if (to <= from)
            {
                new Span<byte>(dstBase, destWidth * destHeight).Fill(fillValue);
                return;
            }

            int copyLen = to - from;
            int srcXOffset = px + from;

            Parallel.For(0, destHeight, row =>
            {
                byte* dst = dstBase + (nint)row * destWidth;
                int donorY = py + row;

                if ((uint)donorY >= (uint)srcHeight)
                {
                    new Span<byte>(dst, destWidth).Fill(fillValue);
                    return;
                }

                if (from > 0)
                    new Span<byte>(dst, from).Fill(fillValue);

                Unsafe.CopyBlockUnaligned(
                    ref *(dst + from),
                    ref *(srcBase + (nint)donorY * srcWidth + srcXOffset),
                    (uint)copyLen);

                if (to < destWidth)
                    new Span<byte>(dst + to, destWidth - to).Fill(fillValue);
            });
        }


        #endregion






    }
}
