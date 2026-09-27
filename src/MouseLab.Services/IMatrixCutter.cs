using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLab.Services
{
    public interface IMatrixCutter
    {
        void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), size, fillZero);
        }

        IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
        {
            return Cut(src, position, new Size(width, height), fillZero);
        }

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), new Size(width, height), fillZero);
        }

        IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), size, provider, fillZero);
        }

        IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, position, new Size(width, height), provider, fillZero);
        }

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true)
        {
            return Cut(src, new Point(x, y), new Size(width, height), provider, fillZero);
        }


    }
}
