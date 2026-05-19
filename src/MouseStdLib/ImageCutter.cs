using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseStdLib
{
    public class ImageCutter : IMatrixCutter
    {
        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), new Size(width, height), fillZero);
        }

        public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
        {
            return Cut(src, position, new Size(width, height), fillZero);
        }

        public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), size, fillZero);
        }

        public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
        {
            if (size.Width == 0 || size.Height == 0)
                return new ImageMatrix(new byte[0, 0]);

            Point dstPosition = new Point(position.X + size.Width, position.Y + size.Height);

            if (dstPosition.X < 0 && dstPosition.X > src.Width && dstPosition.Y < 0 && dstPosition.Y > src.Height)
            {
                byte[] bytes = new byte[size.Width * size.Height];
                Span<byte> bytesSpan = new Span<byte>(bytes);
                foreach (ref byte @byte in bytesSpan)
                {
                    @byte = fillZero ? byte.MinValue : byte.MaxValue;
                }

                return new ImageMatrix(bytes, size);
            }

            byte[,] dst = new byte[size.Height, size.Width];

            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    int newX = x + position.X;
                    int newY = y + position.Y;

                    if (newX < 0 || newX >= src.Width || newY < 0 || newY >= src.Height)
                        dst[y, x] = fillZero ? byte.MinValue : byte.MaxValue;
                    else
                        dst[y, x] = src.At(newX, newY);
                }
            }

            return new ImageMatrix(dst);
        }
    }
}
