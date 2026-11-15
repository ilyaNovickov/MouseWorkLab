using System;
using System.Numerics;
using MouseLabAvaloniaDesktop.Models;

namespace MouseLabAvaloniaDesktop.Providers;

public class ClassicMatrixProvider : IMatrixProvider
{
    public IMatrix Create(int width, int height)
    {
        return new Matrix(width, height);
    }
}
