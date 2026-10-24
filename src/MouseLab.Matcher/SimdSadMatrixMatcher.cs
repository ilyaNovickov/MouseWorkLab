using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Services;

namespace MouseLab.Matcher
{
    /// <summary>
    /// Поиск вектора смещения полным перебором окна ±max по метрике SAD
    /// (sum of absolute differences) на области пересечения.
    ///
    /// Почему перебор, а не трёхшаговый/логарифмический поиск: содержимое
    /// донора — белый шум, метрика на нём «колючая», локальные минимумы
    /// сбивают сокращённые схемы. Полный перебор с ранним отсевом строки и
    /// SIMD дешевле и точнее на ожидаемых размерах (шаблон ≤ 128×128).
    /// </summary>
    public sealed class SimdSadMatrixMatcher : IMatrixMatcher
    {
        // Порог переключения на параллельный перебор: верхняя оценка числа
        // сравниваемых байтов. Ниже него однопоточный SIMD-путь быстрее,
        // чем оплата потоков.
        private const long ParallelCostThreshold = 4_000_000;

        public unsafe Vector Match(IMatrix a, IMatrix b, int maxX, int maxY)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);

            if (a.Width != b.Width || a.Height != b.Height)
                throw new ArgumentException("Матрицы должны совпадать по размеру.");

            if (maxX < 0 || maxY < 0)
                throw new ArgumentException("Окно поиска не может быть отрицательным.");

            // При |dx| >= ширины перекрытие нулевое — сравнивать нечего.
            if (maxX >= a.Width || maxY >= a.Height)
                throw new ArgumentException("Окно поиска должно быть меньше размеров матрицы.");

            int width = a.Width;
            int height = a.Height;

            long cost = (long)(2 * maxX + 1) * (2 * maxY + 1) * width * height;

            fixed (byte* pa = a.ReadOnlySpanBytes, pb = b.ReadOnlySpanBytes)
            {
                if (cost > ParallelCostThreshold)
                    return MatchParallel(pa, pb, width, height, maxX, maxY);

                return MatchSequential(pa, pb, width, height, maxX, maxY);
            }
        }

        private static unsafe Vector MatchSequential(byte* pa, byte* pb, int width, int height, int maxX, int maxY)
        {
            long bestSad = long.MaxValue;
            int bestDx = 0;
            int bestDy = -maxY;

            for (int dy = -maxY; dy <= maxY; dy++)
            {
                for (int dx = -maxX; dx <= maxX; dx++)
                {
                    long sad = CandidateSad(pa, pb, width, height, dx, dy, bestSad);
                    if (sad < bestSad)
                    {
                        bestSad = sad;
                        bestDx = dx;
                        bestDy = dy;
                    }
                }
            }

            return new Vector(bestDx, bestDy);
        }

        private static unsafe Vector MatchParallel(byte* pa, byte* pb, int width, int height, int maxX, int maxY)
        {
            // Строки окна поиска (по dy) независимы; локальный минимум
            // сохраняет порядок перебора dx, а слияние в порядке dy со
            // строгим "<" даёт тот же детерминированный результат, что и
            // однопоточный перебор.
            var rows = new (long Sad, int Dx)[2 * maxY + 1];

            Parallel.For(-maxY, maxY + 1, dy =>
            {
                long bestSad = long.MaxValue;
                int bestDx = -maxX;

                for (int dx = -maxX; dx <= maxX; dx++)
                {
                    long sad = CandidateSad(pa, pb, width, height, dx, dy, bestSad);
                    if (sad < bestSad)
                    {
                        bestSad = sad;
                        bestDx = dx;
                    }
                }

                rows[dy + maxY] = (bestSad, bestDx);
            });

            long best = long.MaxValue;
            int winnerDx = 0;
            int winnerDy = -maxY;

            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Sad < best)
                {
                    best = rows[i].Sad;
                    winnerDx = rows[i].Dx;
                    winnerDy = i - maxY;
                }
            }

            return new Vector(winnerDx, winnerDy);
        }

        /// <summary>
        /// SAD кандидата (dx, dy): сравнение b[x, y] с a[x + dx, y + dy]
        /// на пересечении зон видимости. Если частичная сумма превысила
        /// limit, досрочно выходим — такой кандидат всё равно проиграет.
        /// </summary>
        private unsafe static long CandidateSad(byte* pa, byte* pb, int width, int height, int dx, int dy, long limit)
        {
            int absDx = Math.Abs(dx);
            int absDy = Math.Abs(dy);

            int xStart = Math.Max(0, -dx);
            int yStart = Math.Max(0, -dy);
            int columnLength = width - absDx;
            int rowLength = height - absDy;

            long total = 0;

            for (int y = 0; y < rowLength; y++)
            {
                var rowB = new ReadOnlySpan<byte>(pb + (long)(yStart + y) * width + xStart, columnLength);
                var rowA = new ReadOnlySpan<byte>(pa + (long)(yStart + y + dy) * width + xStart + dx, columnLength);

                total += SadRow(rowB, rowA);

                if (total >= limit)
                    return total;
            }

            return total;
        }

        private static long SadRow(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            if (Vector256.IsHardwareAccelerated && Avx2.IsSupported && left.Length >= Vector256<byte>.Count)
                return SadRowAvx2(left, right);

            return SadRowScalar(left, right);
        }

        private static long SadRowAvx2(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            ref byte l = ref MemoryMarshal.GetReference(left);
            ref byte r = ref MemoryMarshal.GetReference(right);

            // SumAbsoluteDifferences (_mm256_sad_epu8): каждая 64-битная
            // половина вектора копит в младшей 16-битной lane сумму модулей
            // разности 8 байтов. Widen-перегрузки для Vector256<byte> в BCL
            // нет, поэтому аккумулируем этой пер-64-битной суммой, каждые
            // 32 итерации сбрасывая её в long: прирост lane за итерацию —
            // до 8 * 255 = 2040, и 32 такие итерации ushort не переполняют.
            var acc = Vector256<ushort>.Zero;
            int flushGuard = 0;
            int vectorEnd = left.Length & ~(Vector256<byte>.Count - 1);
            long total = 0;

            for (nuint offset = 0; offset < (nuint)vectorEnd; offset += (nuint)Vector256<byte>.Count)
            {
                var va = Vector256.LoadUnsafe(ref l, offset);
                var vb = Vector256.LoadUnsafe(ref r, offset);

                acc = Vector256.Add(acc, Avx2.SumAbsoluteDifferences(va, vb));

                if (++flushGuard == 32)
                {
                    total += SumLanes(acc);
                    acc = Vector256<ushort>.Zero;
                    flushGuard = 0;
                }
            }

            total += SumLanes(acc);

            for (int i = vectorEnd; i < left.Length; i++)
            {
                int d = Unsafe.Add(ref l, i) - Unsafe.Add(ref r, i);
                total += d < 0 ? -d : d;
            }

            return total;
        }

        /// <summary>Сумма всех активных lanes; нечётные у SumAbsoluteDifferences всегда нулевые.</summary>
        private static long SumLanes(Vector256<ushort> acc)
        {
            Span<ushort> lanes = stackalloc ushort[Vector256<ushort>.Count];
            acc.CopyTo(lanes);

            long total = 0;

            for (int i = 0; i < lanes.Length; i += 2)
                total += lanes[i];

            return total;
        }

        private static long SadRowScalar(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            long total = 0;

            for (int i = 0; i < left.Length; i++)
            {
                int d = left[i] - right[i];
                total += (uint)(d < 0 ? -d : d);
            }

            return total;
        }
    }
}
