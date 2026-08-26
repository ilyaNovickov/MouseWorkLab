using MouseBaseLib;

namespace MouseStdLib.Providers
{
    /// <summary>
    /// Провайдер по умолчанию: создаёт обычные <see cref="ImageMatrix"/> с выделением
    /// нового массива под каждую матрицу (поведение, совместимое со старым кодом).
    /// </summary>
    public sealed class DefaultMatrixProvider : IMatrixProvider
    {
        public static readonly DefaultMatrixProvider Instance = new();

        public IMatrix Create(int width, int height)
        {
            return new ImageMatrix(new byte[width * height], width, height, true);
        }
    }
}
