using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    /// <summary>
    /// Интерфейс для поиска движения по 2-м изображениям
    /// </summary>
    public interface IMoveFinder
    {
        /// <summary>
        /// Найти перемещение по 2-м изображениям
        /// </summary>
        /// <param name="matrix1">Матрица №1</param>
        /// <param name="matrix2">Матрица №2</param>
        /// <param name="patchSize">Размер шаблона для поиска</param>
        /// <param name="searchRange">Интервал поиска</param>
        /// <param name="fillZero">Заполнение нулями при выходе за границу матрицы</param>
        /// <returns>Вектор перемещения</returns>
        Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true);
    }
}
