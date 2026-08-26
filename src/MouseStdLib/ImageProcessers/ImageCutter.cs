using MouseBaseLib;
using MouseBaseLib.Interfaces.Services;
using MouseStdLib.Providers;
using System;

namespace MouseStdLib
{
    public class ImageCutter : IMatrixCutter
    {
        public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
            => Cut(src, new Point(x, y), new Size(width, height), fillZero);

        public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
            => Cut(src, position, new Size(width, height), fillZero);

        public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
            => Cut(src, new Point(x, y), size, fillZero);

        public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
            => Cut(src, position, size, DefaultMatrixProvider.Instance, fillZero);

        public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            provider ??= DefaultMatrixProvider.Instance;

            IMatrix result = provider.Create(size.Width, size.Height);

            if (size.Width == 0 || size.Height == 0)
                return result;

            CutInto(src, position.X, position.Y, size.Width, size.Height, fillZero,
                result.RawData.AsSpan(0, size.Width * size.Height));
            return result;
        }

        public void Cut(IMatrix src, Point position, Size size, IMatrix destination, bool fillZero = true)
        {
            if (size.Width != destination.Width || size.Height != destination.Height)
                throw new ArgumentException("Размеры destination должны совпадать с запрошенным размером вырезки.");

            if (size.Width == 0 || size.Height == 0)
                return;

            CutInto(src, position.X, position.Y, size.Width, size.Height, fillZero,
                destination.RawData.AsSpan(0, size.Width * size.Height));
        }

        private static void CutInto(IMatrix src, int px, int py, int w, int h, bool fillZero, Span<byte> dst)
        {
            byte fill = fillZero ? byte.MinValue : byte.MaxValue;

            ReadOnlySpan<byte> srcData = src.GetData();
            int sw = src.Width;
            int sh = src.Height;

            // Построчно: для каждой строки копируем непрерывный сегмент исходника
            // (внутри строки пиксели идут подряд) и заполняем граничные отступы.
            for (int y = 0; y < h; y++)
            {
                int sy = py + y;
                int dstRow = y * w;

                // Вся строка вне исходника — заполняем значением.
                if (sy < 0 || sy >= sh)
                {
                    dst.Slice(dstRow, w).Fill(fill);
                    continue;
                }

                int left = Math.Max(0, -px);      // столбцы с sx < 0
                int right = Math.Min(w, sw - px); // столбцы с sx >= sw

                if (left > 0)
                    dst.Slice(dstRow, left).Fill(fill);

                if (right < w)
                    dst.Slice(dstRow + right, w - right).Fill(fill);

                int count = right - left;
                if (count > 0)
                {
                    int sx = px + left;
                    srcData.Slice(sy * sw + sx, count).CopyTo(dst.Slice(dstRow + left, count));
                }
            }
        }
    }
}
