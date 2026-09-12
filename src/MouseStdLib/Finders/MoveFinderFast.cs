using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseStdLib
{
    public class MoveFinderFast : StdMoveFinder
    {
        public override Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            return Find(matrix1, matrix2, patchSize, searchRange);
        }

        public Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange)
        {
            {
                Exception? exc = ArgsValidation(matrix1, matrix2, patchSize, searchRange);

                if (exc != null)
                    throw new ArgumentException("Ошибка в аргументе", exc);
            }

            //Позыция шаблона
            int patchPositionX = (matrix1.Width - patchSize) / 2;
            int patchPositionY = (matrix1.Height - patchSize) / 2;

            //Диапазон поиска
            //Если шаблон может выйти за границу изображений, то происходит сущение интервала поиска
            int minX = Math.Max(-searchRange, -patchPositionX);
            int maxX = Math.Min(searchRange, matrix1.Width - patchSize - patchPositionX);
            int minY = Math.Max(-searchRange, -patchPositionY);
            int maxY = Math.Min(searchRange, matrix1.Height - patchSize - patchPositionY);

            int minValue = int.MaxValue;
            Vector bestVector = new Vector();

            //Попытка получить быстрый доступ к данным
            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            for (int dy = minY; dy <= maxY; dy++)
            {
                for (int dx = minX; dx <= maxX; dx++)
                {
                    int currentSum = 0;
                    bool isBetter = true;

                    for (int y = 0; y < patchSize; y++)
                    {
                        //Смещение по оси Y для матрицы №1 (шаблон)
                        int y1 = patchPositionY + y;
                        int offset1 = y1 * matrix1.Width;
                        //Смещение по оси Y для матрицы №2
                        int y2 = y1 + dy;
                        int offset2 = y2 * matrix1.Width;

                        for (int x = 0; x < patchSize; x++)
                        {
                            //Смещение по оси X для матрицы №1 (шаблон)
                            int offsetPatchX = patchPositionX + x;
                            //Смещение по оси X для матрицы №2
                            int offsetMovingX = offsetPatchX + dx;

                            byte p1 = data1[offset1 + offsetPatchX];
                            byte p2 = data2[offset2 + offsetMovingX];

                            int diff = p1 - p2;

                            // + ABS(diff)
                            currentSum += (diff < 0) ? -diff : diff;

                            if (currentSum >= minValue)
                            {
                                isBetter = false;
                                break;
                            }
                        }

                        if (!isBetter)
                            break;
                    }

                    if (isBetter && currentSum < minValue)
                    {
                        minValue = currentSum;
                        bestVector = new Vector(dx, dy);
                    }
                }
            }

            return -bestVector;
        }
    }
}
