using System.Collections.Concurrent;

namespace MouseLab.Core.Models
{
    /// <summary>
    /// Пул арендованных PooledMatrix, разбитый по размерам. Потокобезопасен:
    /// аренда и возврат из любых потоков идут через ConcurrentStack.
    /// В установившемся режиме аллокаций нет — одна и та же матрица
    /// (обёртка и буфер вместе) переходит от арендатора к арендатору.
    /// </summary>
    internal sealed class MatrixPool
    {
        private readonly ConcurrentDictionary<(int, int), ConcurrentStack<PooledMatrix>> _stacks = new();

        public PooledMatrix Rent(int width, int height)
        {
            var key = (width, height);

            if (_stacks.TryGetValue(key, out var stack) && stack.TryPop(out var rented))
            {
                rented.Returned = false;
                return rented;
            }

            return new PooledMatrix(width, height) { Owner = this };
        }

        public void Return(PooledMatrix matrix)
        {
            var stack = _stacks.GetOrAdd((matrix.Width, matrix.Height), static _ => new ConcurrentStack<PooledMatrix>());
            stack.Push(matrix);
        }
    }
}
