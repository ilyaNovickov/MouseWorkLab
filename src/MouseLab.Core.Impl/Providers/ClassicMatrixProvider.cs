using System;
using System.Numerics;
using MouseLab.Core.Providers;

namespace MouseLab.Core.Providers;

public class ClassicMatrixProvider : IMatrixProvider
{
    public IMatrix Create(int width, int height)
    {
        return new Matrix(width, height);
    }
}
