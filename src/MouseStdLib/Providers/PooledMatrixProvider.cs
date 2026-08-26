using MouseBaseLib;
using System.Buffers;

namespace MouseStdLib.Providers
{
    /// <summary>
    /// Провайдер, арендующий буферы в <see cref="ArrayPool{T}.Shared"/> через
    /// <see cref="PooledImageMatrix"/>. Снижает давление на GC при частом создании
    /// короткоживущих матриц. Возвращаемые матрицы нужно освобождать через
    /// <see cref="PooledImageMatrix.Dispose"/> (например, через <c>using</c>).
    /// </summary>
    public sealed class PooledMatrixProvider : IMatrixProvider
    {
        public static readonly PooledMatrixProvider Instance = new();

        public IMatrix Create(int width, int height)
        {
            int length = width * height;

            if (length == 0)
                return new ImageMatrix(System.Array.Empty<byte>(), 0, 0, true);

            byte[] rented = ArrayPool<byte>.Shared.Rent(length);

            // Cut/Randomize полностью перезаписывают область width*height,
            // поэтому явная очистка арендованного буфера не требуется.
            return new PooledImageMatrix(rented, width, height);
        }
    }
}
