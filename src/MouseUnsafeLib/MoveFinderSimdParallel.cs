using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Num = System.Numerics;

namespace MouseUnsafeLib
{
    public class MoveFinderSimdParallel : MoveFinderSimdBase
    {
        public override Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            return Find(matrix1, matrix2, patchSize, searchRange);
        }

        [SkipLocalsInit]
        public unsafe Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange)
        {
            int patchPositionX = (matrix1.Width - patchSize) / 2;
            int patchPositionY = (matrix1.Height - patchSize) / 2;

            int minX = Math.Max(-searchRange, -patchPositionX);
            int maxX = Math.Min(searchRange, matrix1.Width - patchSize - patchPositionX);
            int minY = Math.Max(-searchRange, -patchPositionY);
            int maxY = Math.Min(searchRange, matrix1.Height - patchSize - patchPositionY);

            int width = matrix1.Width;
            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            // Объекты для хранения итогового результата
            object syncLock = new object();
            int globalMinValue = int.MaxValue;
            Vector globalBestVector = new Vector(0, 0);

            fixed (byte* pData1 = data1)
            fixed (byte* pData2 = data2)
            {
                // Преобразуем указатели в системные целые числа (адреса)
                nint addr1 = (nint)pData1;
                nint addr2 = (nint)pData2;

                // Параллельный цикл по вертикальному смещению
                Parallel.For(
                    minY,           // От
                    maxY + 1,       // До (исключая)

                    // Локальная инициализация для каждого потока
                    () => (LocalMin: int.MaxValue, LocalBest: new Vector(0, 0)),

                    // Тело цикла (dy - текущая итерация, state - управление, localData - данные потока)
                    (dy, state, localData) =>
                    {
                        byte* ptr1 = (byte*)addr1;
                        byte* ptr2 = (byte*)addr2;

                        for (int dx = minX; dx <= maxX; dx++)
                        {
                            int currentSum = 0;
                            bool isBetter = true;

                            for (int y = 0; y < patchSize; y++)
                            {
                                int y1 = patchPositionY + y;
                                int y2 = y1 + dy;

                                byte* row1 = ptr1 + (y1 * width + patchPositionX);
                                byte* row2 = ptr2 + (y2 * width + patchPositionX + dx);

                                currentSum += CalculateSAD(row1, row2, patchSize);

                                // Сравниваем с локальным минимумом потока
                                if (currentSum >= localData.LocalMin)
                                {
                                    isBetter = false;
                                    break;
                                }
                            }

                            if (isBetter)
                            {
                                localData.LocalMin = currentSum;
                                localData.LocalBest = new Vector(dx, dy);
                            }
                        }
                        return localData;
                    },

                    // Финализация: объединение результатов потоков
                    (finalLocalData) =>
                    {
                        lock (syncLock)
                        {
                            if (finalLocalData.LocalMin < globalMinValue)
                            {
                                globalMinValue = finalLocalData.LocalMin;
                                globalBestVector = finalLocalData.LocalBest;
                            }
                        }
                    }
                );
            }

            return -globalBestVector;
        }
    }
}
