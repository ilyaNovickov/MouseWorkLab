using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    /// <summary>
    /// Интерфейс для создания случайно матрицы
    /// </summary>
    public interface IMatrixRandomizer
    {
        /// <summary>
        /// Создание случайно матрицы
        /// </summary>
        /// <param name="Width">Ширина</param>
        /// <param name="Height">Высота</param>
        /// <param name="seed">Сид</param>
        /// <returns>Случайная матрицы</returns>
        IMatrix Randomize(int Width, int Height, int? seed = null);
        /// <summary>
        /// Создание случайно матрицы
        /// </summary>
        /// <param name="size">Размеры матрицы</param>
        /// <param name="seed">Сид</param>
        /// <returns>Случайная матрицы</returns>
        IMatrix Randomize(Size size, int? seed = null);

        /// <summary>
        /// Создаёт случайную матрицу через указанный провайдер (например,
        /// <see cref="MouseStdLib.Providers.PooledMatrixProvider"/> для пулинга).
        /// </summary>
        IMatrix Randomize(int Width, int Height, IMatrixProvider provider, int? seed = null);
        /// <summary>
        /// Создаёт случайную матрицу через указанный провайдер (например,
        /// <see cref="MouseStdLib.Providers.PooledMatrixProvider"/> для пулинга).
        /// </summary>
        IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null);
        /// <summary>
        /// Заполнение случайными значениями указанную матрицу
        /// </summary>
        void Randomize(IMatrix dest, int Width, int Height, int? seed = null);
    }
}
