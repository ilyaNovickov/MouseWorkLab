using MouseBaseLib;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace MouseStdLib
{
    public class ImageMatrix : IMatrix
    {
        private byte[] matrix;
        private Size size;

        public ImageMatrix(byte[] matrix, Size size)
        {
            this.matrix = (byte[])matrix.Clone();
            this.size = size;
        }

        public ImageMatrix(byte[] matrix, int width, int height) : this(matrix, new Size(width, height))
        {

        }

        public ImageMatrix(byte[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);
            this.size = new Size(width, height);
            this.matrix = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                Buffer.BlockCopy(matrix, y * width, this.matrix, y * width, width);
            }
        }

        public int Width => size.Width;

        public int Height => size.Height;

        public byte[] RawData => matrix;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte At(int x, int y)
        {
            return ref matrix[y * Width + x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref byte At(Point point)
        {
            return ref At(point.X, point.Y);
        }

        public ref byte AtWithCheck(int x, int y)
        {
            if (0 > y || y >= Height)
                throw new ArgumentException("Выход за границы по высоте матрицы");

            if (0 > x || x >= Width)
                throw new ArgumentException("Выход за границы по ширине матрицы");

            return ref At(x, y);
        }

        public ref byte AtWithCheck(Point point)
        {
            return ref AtWithCheck(point.X, point.Y);
        }

        public ReadOnlySpan<byte> GetData()
        {
            return matrix.AsSpan<byte>();
        }

        public ReadOnlySpan<byte> GetRow(int y)
        {
            return matrix.AsSpan<byte>(y * Width, Width);
        }

        public string ToString(string arg)
        {
            if (arg.ToLower() is not ("d" or "debug"))
                return this.ToString()!;

            const int maxLen = 3;
            int rowLength = Width * (maxLen + 1) + Environment.NewLine.Length;
            StringBuilder stringBuilder = new StringBuilder(rowLength * Height);

            for (int y = 0; y < Height; y++)
            {
                ReadOnlySpan<byte> row = this.GetRow(y);

                for (int i = 0; i < row.Length; i++)
                {
                    byte value = row[i];
                    int len = value < 10 ? 1 : value < 100 ? 2 : 3;
                    int diff = maxLen - len;

                    if (diff > 0)
                        stringBuilder.Append(' ', diff);

                    stringBuilder.Append(value);
                    stringBuilder.Append('|');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
