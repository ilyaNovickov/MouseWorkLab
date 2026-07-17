using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseStdLib
{
    public class MoveFinderBoundy : StdMoveFinder
    {
        public override Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true)
        {
            {
                Exception? exc = ArgsValidation(matrix1, matrix2, patchSize, searchRange);

                if (exc != null)
                    throw new ArgumentException("Ошибка в аргументе", exc);
            }

            //Позыция шаблона
            int patchPositionX = (matrix1.Width - patchSize) / 2;
            int patchPositionY = (matrix1.Height - patchSize) / 2;

            // Значение для пикселей за границей
            byte outOfBoundsValue = fillZero ? byte.MinValue : byte.MaxValue;

            // Попытка получить быстрый доступ к данным
            ReadOnlySpan<byte> data1 = matrix1.GetData();
            ReadOnlySpan<byte> data2 = matrix2.GetData();

            int minValue = int.MaxValue;
            Vector bestVector = new Vector();

            for (int dy = -searchRange; dy <= searchRange; dy++)
            {
                for (int dx = -searchRange; dx <= searchRange; dx++)
                {
                    //Проверка выхода за границу изображения
                    bool isOutOfBounds = (patchPositionX + dx < 0) || (patchPositionX + dx + patchSize > matrix1.Width) ||
                                         (patchPositionY + dy < 0) || (patchPositionY + dy + patchSize > matrix1.Height);

                    int currentSum = 0;
                    bool isBetter = true;

                    //Пути при выходе за границу изображения и при полном вхождении в изображение
                    //мало отличаются, но как это переписать по DRY - я не знаю
                    if (isOutOfBounds)
                    {
                        //Медленный путь: заполнения пустого пространства
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
                                int offsetPatchX = patchPositionX + x;
                                int offsetMovingX = offsetPatchX + dx;

                                byte p1, p2;

                                // matrix1 всегда в пределах (патч берётся из центра)
                                p1 = data1[offset1 + offsetPatchX];


                                // matrix2 может выходить за границы
                                if (offsetMovingX < 0 || offsetMovingX >= matrix1.Width || y2 < 0 || y2 >= matrix1.Height)
                                    p2 = outOfBoundsValue;
                                else 
                                    p2 = data2[offset2 + offsetMovingX];

                                int diff = p1 - p2;
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
                    }
                    else
                    {
                        //Быстрый путь: нет вывхода за границу
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
