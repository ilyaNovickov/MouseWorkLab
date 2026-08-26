using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    public interface IMatrixCutter
    {
        IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true);

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
