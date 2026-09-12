namespace MouseBaseLib;

/// <summary>
/// Интерфейс 2D матрицы
/// </summary>
public interface IMatrix : IDisposable
{
    /// <summary>
    /// Ширина матрицы
    /// </summary>
    int Width { get; }
    /// <summary>
    /// Высота матрицы
    /// </summary>
    int Height { get; }
    /// <summary>
    /// Сырые данные матрицы
    /// </summary>
    byte[] RawData { get; }
    /// <summary>
    /// Ссылка на элемент матрицы
    /// </summary>
    /// <param name="x">Значение X</param>
    /// <param name="y">Значение Y</param>
    /// <returns>Ссылка на элемент матрицы</returns>
    ref byte At(int x, int y);
    /// <summary>
    /// Ссылка на элемент матрицы
    /// </summary>
    /// <param name="point">Точка координат</param>
    /// <returns>Ссылка на элемент матрицы</returns>
    ref byte At(Point point);
    /// <summary>
    /// Ссылка на элемент матрицы (с проверкой на выход за границы матрицы)
    /// </summary>
    /// <param name="x">Значение X</param>
    /// <param name="y">Значение Y</param>
    /// <returns>Ссылка на элемент матрицы</returns>
    ref byte AtWithCheck(int x, int y);
    /// <summary>
    /// Ссылка на элемент матрицы (с проверкой на выход за границы матрицы)
    /// </summary>
    /// <param name="point">Точка координат</param>
    /// <returns>Ссылка на элемент матрицы</returns>
    ref byte AtWithCheck(Point point);
    /// <summary>
    /// Получение сырых данных
    /// </summary>
    /// <returns>Массив сырых данных</returns>
    ReadOnlySpan<byte> GetData();
    /// <summary>
    /// Получение строки сырых данных
    /// </summary>
    /// <param name="y">Номер строки</param>
    /// <returns>Сырые данные строки матрицы</returns>
    ReadOnlySpan<byte> GetRow(int y);
}
