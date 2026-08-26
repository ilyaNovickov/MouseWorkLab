using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib.Interfaces.Services
{
    public interface IMatrixRandomizer
    {
        IMatrix Randomize(int Width, int Height, int? seed = null);
        IMatrix Randomize(Size size, int? seed = null);

        /// <summary>
        /// Создаёт случайную матрицу через указанный провайдер (например,
        /// <see cref="MouseStdLib.Providers.PooledMatrixProvider"/> для пулинга).
        /// </summary>
        IMatrix Randomize(int Width, int Height, IMatrixProvider provider, int? seed = null);
        IMatrix Randomize(Size size, IMatrixProvider provider, int? seed = null);
    }
}
