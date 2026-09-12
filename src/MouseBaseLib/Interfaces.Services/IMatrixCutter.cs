using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{ 
    /// <summary>
    /// Интерфейс вырезания участка матрицы
    /// </summary>
    public interface IMatrixCutter
    {
        /// <summary>
        /// Вырезает область матрицы
        /// </summary>
        /// <param name="src">Исходная матрица</param>
        /// <param name="position">Координаты для вырезания</param>
        /// <param name="size">Размер вырезаемой области</param>
        /// <param name="fillZero">Заполнение нулями при выходе за границы исходной матрицы</param>
        /// <returns>Вырезанная область</returns>
        IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true);
        /// <summary>
        /// Вырезает область матрицы
        /// </summary>
        /// <param name="src">Исходная матрица</param>
        /// <param name="x">Позыция по оси X</param>
        /// <param name="y">Позыция по оси Y</param>
        /// <param name="width">Ширина области</param>
        /// <param name="height">Высота области</param>
        /// <param name="fillZero">Заполнение нулями при выходе за границы исходной матрицы</param>
        /// <returns>Вырезанная область</returns>
        IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true);
        /// <summary>
        /// Вырезает область матрицы
        /// </summary>
        /// <param name="src">Исходная матрица</param>
        /// <param name="position">Координаты для вырезания</param>
        /// <param name="width">Ширина области</param>
        /// <param name="height">Высота области</param>
        /// <param name="fillZero">Заполнение нулями при выходе за границы исходной матрицы</param>
        /// <returns>Вырезанная область</returns>
        IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true);
        /// <summary>
        /// Вырезает область матрицы
        /// </summary>
        /// <param name="src">Исходная матрица</param>
        /// <param name="x">Позыция по оси X</param>
        /// <param name="y">Позыция по оси Y</param>
        /// <param name="size">Размер вырезаемой области</param>
        /// <param name="fillZero">Заполнение нулями при выходе за границы исходной матрицы</param>
        /// <returns>Вырезанная область</returns>
        IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true);

        /// <summary>
        /// Вырезает область, используя указанный провайдер для создания результата
        /// (например, <see cref="MouseStdLib.Providers.PooledMatrixProvider"/> для пулинга).
        /// </summary>
        IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true);

        /// <summary>
        /// Вырезает область непосредственно в уже существующую матрицу <paramref name="destination"/>
        /// (её размеры должны совпадать с запрошенным). Позволяет полностью исключить аллокации.
        /// </summary>
        void Cut(IMatrix src, Point position, Size size, IMatrix destination, bool fillZero = true);
    }
}
