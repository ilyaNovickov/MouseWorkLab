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
                return new ImageMatrix([], 0, 0, true);

            Point dstPosition = new Point(position.X + size.Width, position.Y + size.Height);
            byte fillValue = fillZero ? byte.MinValue : byte.MaxValue;

            if ((dstPosition.X < 0 || dstPosition.X > src.Width) && (dstPosition.Y < 0 || dstPosition.Y > src.Height))
            {
                byte[] bytes = new byte[size.Width * size.Height];
                
                bytes.AsSpan().Fill(fillValue);

                return new ImageMatrix(bytes, size, true);
            }

            int totalPixels = size.Width * size.Height;
            byte[] dst = new byte[totalPixels];

            for (int i = 0; i < totalPixels; i++)
            {
                int x = i % size.Width;
                int y = i / size.Width;
                int newX = x + position.X;
                int newY = y + position.Y;

                if (newX < 0 || newX >= src.Width || newY < 0 || newY >= src.Height)
                    dst[i] = fillValue;
                else
                    dst[i] = src.At(newX, newY);
            }

            return new ImageMatrix(dst, size, true);
        }
    }
}
