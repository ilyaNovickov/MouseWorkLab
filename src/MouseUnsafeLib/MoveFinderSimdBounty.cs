using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MouseUnsafeLib
{
    public class MoveFinderSimdBounty : MoveFinderSimdBase
    {
        public override unsafe Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            {
                Exception? exc = ArgsValidation(matrix1, matrix2, patchSize, searchRange);

                if (exc != null)
                    throw new ArgumentException("Ошибка в аргументе", exc);
            }

            int width = matrix1.Width;
            int height = matrix1.Height;
            int patchPositionX = (width - patchSize) / 2;
            int patchPositionY = (height - patchSize) / 2;
            byte outOfBoundsValue = fillZero ? (byte)0 : (byte)255;

            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            int minValue = int.MaxValue;
            Vector bestVector = new Vector();

            // Создаем вектор из константы заполнения для SIMD
            var vFill = Vector256.Create(outOfBoundsValue);
            var vFill128 = Vector128.Create(outOfBoundsValue);

            fixed (byte* pData1 = data1)
            fixed (byte* pData2 = data2)
            {
                for (int dy = -searchRange; dy <= searchRange; dy++)
                {
                    for (int dx = -searchRange; dx <= searchRange; dx++)
                    {
                        int currentSum = 0;
                        bool isBetter = true;

                        // Определяем границы для текущего смещения dx/dy
                        int currentMovingY = patchPositionY + dy;
                        int currentMovingX = patchPositionX + dx;

                        for (int y = 0; y < patchSize; y++)
                        {
                            int y1 = patchPositionY + y;
                            int y2 = currentMovingY + y;

                            byte* row1 = pData1 + (y1 * width + patchPositionX);

                            // СЛУЧАЙ 1: Строка целиком вне изображения по вертикали
                            if (y2 < 0 || y2 >= height)
                            {
                                currentSum += CalculateSADWithConstant(row1, outOfBoundsValue, patchSize);
                            }
                            else
                            {
                                // СЛУЧАЙ 2: Строка внутри по вертикали, проверяем горизонталь
                                if (currentMovingX >= 0 && currentMovingX + patchSize <= width)
                                {
                                    // Подслучай А: Строка целиком внутри (самый быстрый путь)
                                    byte* row2 = pData2 + (y2 * width + currentMovingX);
                                    currentSum += CalculateSAD(row1, row2, patchSize);
                                }
                                else
                                {
                                    // Подслучай Б: Строка частично выходит за границы по горизонтали
                                    currentSum += CalculateSADPartiallyOutOfBounds(row1, pData2, y2, currentMovingX, patchSize, width, outOfBoundsValue);
                                }
                            }

                            if (currentSum >= minValue)
                            {
                                isBetter = false;
                                break;
                            }
                        }

                        if (isBetter && currentSum < minValue)
                        {
                            minValue = currentSum;
                            bestVector = new Vector(dx, dy);
                        }
                    }
                }
            }

            return -bestVector;
        }

        // SIMD SAD когда одна сторона - константа (выход за границы)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe static int CalculateSADWithConstant(byte* p1, byte constant, int length)
        {
            int sum = 0;
            int i = 0;
            var vConst256 = Vector256.Create(constant);
            var vConst128 = Vector128.Create(constant);

            if (Avx2.IsSupported && length >= 32)
            {
                Vector256<ulong> vSum = Vector256<ulong>.Zero;
                for (; i <= length - 32; i += 32)
                    vSum = Avx2.Add(vSum, Avx2.SumAbsoluteDifferences(Avx.LoadVector256(p1 + i), vConst256).AsUInt64<ushort>());
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1) + vSum.GetElement(2) + vSum.GetElement(3));
            }
            else if (Sse2.IsSupported && length >= 16)
            {
                Vector128<ulong> vSum = Vector128<ulong>.Zero;
                for (; i <= length - 16; i += 16)
                    vSum = Sse2.Add(vSum, Sse2.SumAbsoluteDifferences(Sse2.LoadVector128(p1 + i), vConst128).AsUInt64<ushort>());
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1));
            }
            for (; i < length; i++) sum += Math.Abs(p1[i] - constant);
            return sum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static unsafe int CalculateSADPartially(byte* row1, byte* pData2, int y2, int startX, int patchSize, int width, byte fillValue, byte* pTempRow)
        {
            // Быстрая очистка временного буфера текущим значением фона
            new Span<byte>(pTempRow, patchSize).Fill(fillValue);

            // Вычисляем область пересечения
            int imageXStart = Math.Max(0, startX);
            int imageXEnd = Math.Min(width, startX + patchSize);
            int validWidth = imageXEnd - imageXStart;

            if (validWidth > 0)
            {
                int offsetInPatch = imageXStart - startX;
                byte* src = pData2 + (y2 * width + imageXStart);
                byte* dst = pTempRow + offsetInPatch;
                // Копируем только то, что попало в изображение
                Buffer.MemoryCopy(src, dst, validWidth, validWidth);
            }

            // После сборки строки используем стандартный SIMD SAD
            return CalculateSAD(row1, pTempRow, patchSize);
        }

        // Обработка строки, которая частично вылезла за левый или правый край
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe static int CalculateSADPartiallyOutOfBounds(byte* row1, byte* pData2, int y2, int startX, int patchSize, int width, byte fillValue)
        {
            // Используем stackalloc для временной сборки строки (быстро, без аллокаций в куче)
            // Ограничим разумным пределом, например 2048 пикселей.
            byte* tempRow = stackalloc byte[patchSize];

            return CalculateSADPartially(row1, pData2, y2, startX, patchSize, width, fillValue, tempRow);
        }
    }
}
