using MouseLab.Core;
using MouseLab.Core.Models;

namespace MouseLab.Services
{
    /// <summary>
    /// Определяет вектор смещения между двумя фрагментами, вырезанными
    /// из одного и того же донора, по содержимому самих фрагментов.
    /// </summary>
    /// <remarks>
    /// Контракт: возвращает вектор <c>v</c>, для которого
    /// <c>b[x, y] == a[x + v.Dx, y + v.Dy]</c> на области перекрытия.
    /// Например, фрагмент №1 вырезан в (0,0), фрагмент №2 — в (1,1):
    /// результат Match(№1, №2) равен (1, 1).
    /// Совпадение не гарантируется: при слабом перекрытии или повторяющемся
    /// содержимом донора возможен ложный минимум.
    /// </remarks>
    public interface IMatrixMatcher
    {
        /// <summary>
        /// Ищет вектор смещения в окне [-maxX..maxX] × [-maxY..maxY].
        /// </summary>
        /// <exception cref="ArgumentNullException">Матрица null.</exception>
        /// <exception cref="ArgumentException">
        /// Размеры матриц не совпадают либо |maxX| >= ширины или
        /// |maxY| >= высоты: перекрытия нет, искать нечего.
        /// </exception>
        Vector Match(IMatrix a, IMatrix b, int maxX, int maxY);

        Vector Match(IMatrix a, IMatrix b, Vector max)
        {
            return Match(a, b, max.Dx, max.Dy);
        }

        Vector Match(IMatrix a, IMatrix b, int max)
        {
            return Match(a, b, max, max);
        }
    }
}
