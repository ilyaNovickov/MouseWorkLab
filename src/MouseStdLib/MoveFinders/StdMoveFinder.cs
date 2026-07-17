using MouseBaseLib;
using System.Runtime.CompilerServices;
using Num = System.Numerics;

namespace MouseStdLib
{
    public abstract class StdMoveFinder : IMoveFinder
    {
        protected static Exception? ArgsValidation(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange)
        {
            if (matrix1 == null || matrix2 == null)
                return new ArgumentException("Должны быть хоть какие-то изображения");

            if (matrix1.Width == 0 || matrix1.Height == 0 || matrix2.Width == 0 || matrix2.Height == 0)
                return new ArgumentException("Изображения имеют нулевой размер");

            if (matrix1.Width != matrix2.Width || matrix1.Height != matrix2.Height)
                return new ArgumentException("Изображения должны быть одинакового размера");

            if (matrix1.Width != matrix1.Height)
                return new Exception("Изображения должны быть квадратными");

            if (patchSize > matrix1.Width || patchSize > matrix1.Height)
                return new ArgumentException("Шаблон больше изображения");

            if (searchRange <= 0)
                return new ArgumentException("Интервал поиска должен быть больше 0");

            return null;
        }

        public abstract Vector Find(IMatrix matrix1, IMatrix matrix2, int patchSize, int searchRange, bool fillZero = true);
    }
}
