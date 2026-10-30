using System;
using System.Numerics;
using MouseLab.Core.Providers;
using MouseLab.Core.Models;

namespace MouseLab.Common.Providers;

public class ClassicMatrixProvider : IMatrixProvider
{
    public IMatrix Create(int width, int height)
    {
        return new Matrix(width, height);
    }
}
