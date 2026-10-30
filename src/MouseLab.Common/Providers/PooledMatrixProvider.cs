using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using MouseLab.Common.Models.PooledMatrix;

namespace MouseLab.Common.Providers
{
    /// <summary>
    /// Реализация IMatrixProvider: Create отдаёт матрицу из внутреннего пула,
    /// поэтому повторные вставки одного и того же размера не выделяют память.
    /// Освобождение происходит при Dispose матрицы.
    /// </summary>
    public sealed class PooledMatrixProvider : IMatrixProvider
    {
        private readonly MatrixPool _pool = new();

        public IMatrix Create(int width, int height) => _pool.Rent(width, height);
    }
}
