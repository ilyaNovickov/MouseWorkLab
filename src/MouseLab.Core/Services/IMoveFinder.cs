using System;
using MouseLab.Core.Models;

namespace MouseLab.Core.Services;

public interface IMoveFinder
{
    Vector Find(IMatrix a, IMatrix b, int patchSize, int searchRange, bool fillZero = false);
}
