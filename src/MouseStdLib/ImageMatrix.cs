using MouseBaseLib;
using System.Text;

namespace MouseStdLib
{
    public class ImageMatrix : IMatrix
    {
        private byte[] matrix;
        private Size size;
        
        public ImageMatrix(byte[] matrix, Size size)
        {
            this.matrix = (byte[]) matrix.Clone();
            this.size = size;
        }

        public ImageMatrix(byte[,] matrix)
        {
            this.size = new Size(matrix.GetLength(0), matrix.GetLength(1));
            this.matrix = new byte[matrix.Length];
            matrix.CopyTo(this.matrix, 0);
        }

        public int Width => size.Width;

        public int Height => size.Height;

        public byte[] RawData => matrix;

        public ref byte At(int x, int y)
        {
            return ref matrix[y * Width + x];
        }

        public ref byte AtWithCheck(int x, int y)
        {
            if (0 > y || y >= Height)
                throw new ArgumentException("Выход за границы по высоте матрицы");

            if (0 > x || x >= Width)
                throw new ArgumentException("Выход за границы по ширине матрицы");

            return ref At(x, y);
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

            StringBuilder builder = new();

            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                ReadOnlySpan<byte> row = this.GetRow(y);

                foreach (byte value in row)
                {
                    StringBuilder inner = new(3);
                    inner.Append(value);

                    //Выравнивание byte по левому краю
                    int diff = maxLen - inner.Length;

                    if (diff != 0)
                    {
                        inner.Insert(0, new string(' ', diff));
                    }

                    stringBuilder.Append(inner);
                    stringBuilder.Append('|');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
