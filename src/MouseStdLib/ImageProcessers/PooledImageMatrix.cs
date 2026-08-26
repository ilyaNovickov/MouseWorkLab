using MouseBaseLib;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace MouseStdLib
{
    /// <summary>
    /// Реализация <see cref="IMatrix"/>, арендующая буфер в <see cref="ArrayPool{T}.Shared"/>.
    /// После использования матрицу обязательно нужно освободить через <see cref="Dispose"/>,
    /// чтобы буфер вернулся в пул. После освобождения вызовы <c>GetData</c>, <c>RawData</c>,
    /// <c>At</c> недействительны.
    /// </summary>
    public sealed class PooledImageMatrix : IMatrix, IDisposable
    {
        private readonly byte[] _rented;
        private readonly Size _size;
        private bool _disposed;

        internal PooledImageMatrix(byte[] rented, int width, int height)
        {
            _rented = rented;
            _size = new Size(width, height);
        }

        public int Width => _size.Width;

        public int Height => _size.Height;

        // Возвращает арендованный буфер целиком (его длина может превышать width*height).
        // Для точных данных используйте GetData()/GetRow().
        public byte[] RawData
        {
            get
            {
                ThrowIfDisposed();
                return _rented;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte At(int x, int y)
        {
            ThrowIfDisposed();
            return ref _rented[y * Width + x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte At(Point point)
        {
            return ref At(point.X, point.Y);
        }

        public ref byte AtWithCheck(int x, int y)
        {
            ThrowIfDisposed();

            if (0 > y || y >= Height)
                throw new System.ArgumentException("Выход за границы по высоте матрицы");

            if (0 > x || x >= Width)
                throw new System.ArgumentException("Выход за границы по ширине матрицы");

            return ref At(x, y);
        }

        public ref byte AtWithCheck(Point point)
        {
            return ref AtWithCheck(point.X, point.Y);
        }

        public ReadOnlySpan<byte> GetData()
        {
            ThrowIfDisposed();
            return _rented.AsSpan(0, Width * Height);
        }

        public ReadOnlySpan<byte> GetRow(int y)
        {
            ThrowIfDisposed();
            return _rented.AsSpan(y * Width, Width);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            ArrayPool<byte>.Shared.Return(_rented, clearArray: false);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new System.ObjectDisposedException(nameof(PooledImageMatrix));
        }
    }
}
