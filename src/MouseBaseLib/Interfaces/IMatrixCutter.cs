using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public interface IMatrixCutter
    {
        IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true);
    }
}
