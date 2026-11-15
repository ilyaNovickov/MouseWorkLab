
using System;
using System.Collections.Generic;
using System.Text;
using MouseLabAvaloniaDesktop.Models;
using MouseLabAvaloniaDesktop.Providers;

namespace MouseLabAvaloniaDesktop.Services.Abstractions
{
    public interface IMatrixCutter
    {
        void Cut(IMatrix src, int x, int y, int width, int height, IMatrix dest, bool fillZero = true);
        void Cut(IMatrix src, int x, int y, Size size, IMatrix dest, bool fillZero = true);
        void Cut(IMatrix src, Point position, int width, int height, IMatrix dest, bool fillZero = true);
        void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true);

        IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true);

        IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true);


    }
}
