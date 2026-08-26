namespace MouseBaseLib;

/// <summary>
/// Абстракция для создания экземпляров <see cref="IMatrix"/>.
/// Позволяет подменять стратегию выделения памяти (обычная аллокация или пулинг).
/// </summary>
public interface IMatrixProvider
{
    /// <summary>
    /// Создаёт матрицу заданного размера. Реализация гарантирует, что вся область
    /// памяти (ширина * высота байт) доступна для записи и последующего чтения.
    /// </summary>
    IMatrix Create(int width, int height);
}
