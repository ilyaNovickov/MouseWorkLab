using MouseBaseLib;
using MouseStdLib;
using Num = System.Numerics;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace MouseUnsafeLib.Finders
{
    public class MoveFinderSimd : MoveFinderSimdBase
    {
        public override Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            return Find(matrix1, matrix2, patchSize, searchRange);
        }

        public unsafe Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange)
        {
            {
                Exception? exc = ArgsValidation(matrix1, matrix2, patchSize, searchRange);

                if (exc != null)
                    throw new ArgumentException("Ошибка в аргументе", exc);
            }

            // Позиция шаблона (центр)
            int patchPositionX = (matrix1.Width - patchSize) / 2;
            int patchPositionY = (matrix1.Height - patchSize) / 2;

            int minX = Math.Max(-searchRange, -patchPositionX);
            int maxX = Math.Min(searchRange, matrix1.Width - patchSize - patchPositionX);
            int minY = Math.Max(-searchRange, -patchPositionY);
            int maxY = Math.Min(searchRange, matrix1.Height - patchSize - patchPositionY);

            int minValue = int.MaxValue;
            Vector bestVector = new Vector(0, 0);

            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            // Фиксируем указатели, чтобы избежать проверок границ Span в циклах
            fixed (byte* pData1 = data1)
            fixed (byte* pData2 = data2)
            {
                for (int dy = minY; dy <= maxY; dy++)
                {
                    for (int dx = minX; dx <= maxX; dx++)
                    {
                        int currentSum = 0;

                        for (int y = 0; y < patchSize; y++)
                        {
                            //Смещение по оси Y для матрицы №1 (шаблон)
                            int y1 = patchPositionY + y;
                            //Смещение по оси Y для матрицы №2
                            int y2 = y1 + dy;

                            byte* row1 = pData1 + (y1 * matrix1.Width + patchPositionX);
                            byte* row2 = pData2 + (y2 * matrix1.Width + patchPositionX + dx);

                            currentSum += CalculateSAD(row1, row2, patchSize);

                            if (currentSum >= minValue) 
                                break;
                        }

                        if (currentSum < minValue)
                        {
                            minValue = currentSum;
                            bestVector = new Vector(dx, dy);
                        }
                    }
                }
            }

            return -bestVector;
        }
    }
}
