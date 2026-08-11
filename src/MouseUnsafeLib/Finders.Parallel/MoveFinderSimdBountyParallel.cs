using MouseBaseLib;
using MouseUnsafeLib.Finders;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MouseUnsafeLib
{
    public class MoveFinderSimdBountyParallel : MoveFinderSimdBounty
    {
        public override unsafe Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            {
                Exception? exc = ArgsValidation(matrix1, matrix2, patchSize, searchRange);

                if (exc != null)
                    throw new ArgumentException("Ошибка в аргументе", exc);
            }

            // 1. Валидация и начальные расчеты
            int width = matrix1.Width;
            int height = matrix1.Height;
            int patchPositionX = (width - patchSize) / 2;
            int patchPositionY = (height - patchSize) / 2;
            byte outOfBoundsValue = fillZero ? (byte)0 : (byte)255;

            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            object syncLock = new object();
            int globalMinValue = int.MaxValue;
            Vector globalBestVector = new Vector();

            // 2. Фиксация памяти
            fixed (byte* pData1 = data1)
            fixed (byte* pData2 = data2)
            {
                nint addr1 = (nint)pData1;
                nint addr2 = (nint)pData2;

                // 3. Параллельный цикл по смещению Y (dy)
                Parallel.For(
                    -searchRange,
                    searchRange + 1,
                    // Инициализация локальных данных для каждого потока
                    () => (Min: int.MaxValue, Best: new Vector(), Buffer: new byte[patchSize]),
                    (dy, state, local) =>
                    {
                        byte* ptr1 = (byte*)addr1;
                        byte* ptr2 = (byte*)addr2;

                        // Фиксируем временный буфер внутри потока, чтобы не аллоцировать в циклах
                        fixed (byte* pTempRow = local.Buffer)
                        {
                            for (int dx = -searchRange; dx <= searchRange; dx++)
                            {
                                int currentSum = 0;
                                int currentMovingY = patchPositionY + dy;
                                int currentMovingX = patchPositionX + dx;
                                bool isBetter = true;

                                for (int y = 0; y < patchSize; y++)
                                {
                                    int y1 = patchPositionY + y;
                                    int y2 = currentMovingY + y;

                                    byte* row1 = ptr1 + (y1 * width + patchPositionX);

                                    // Логика выбора метода SAD (SIMD внутри каждого)
                                    if (y2 < 0 || y2 >= height)
                                    {
                                        // Полный выход за границы по Y
                                        currentSum += CalculateSADWithConstant(row1, outOfBoundsValue, patchSize);
                                    }
                                    else if (currentMovingX >= 0 && currentMovingX + patchSize <= width)
                                    {
                                        // Полностью внутри границ
                                        byte* row2 = ptr2 + (y2 * width + currentMovingX);
                                        currentSum += CalculateSAD(row1, row2, patchSize);
                                    }
                                    else
                                    {
                                        // Частичный выход за границы по X
                                        currentSum += CalculateSADPartially(row1, ptr2, y2, currentMovingX, patchSize, width, outOfBoundsValue, pTempRow);
                                    }

                                    if (currentSum >= local.Min)
                                    {
                                        isBetter = false;
                                        break;
                                    }
                                }

                                if (isBetter)
                                {
                                    local.Min = currentSum;
                                    local.Best = new Vector(dx, dy);
                                }
                            }
                        }
                        return local;
                    },
                    // Финализация локальных результатов потока
                    (finalLocal) =>
                    {
                        lock (syncLock)
                        {
                            if (finalLocal.Min < globalMinValue)
                            {
                                globalMinValue = finalLocal.Min;
                                globalBestVector = finalLocal.Best;
                            }
                        }
                    }
                );
            }

            return -globalBestVector;
        }
    }
}
