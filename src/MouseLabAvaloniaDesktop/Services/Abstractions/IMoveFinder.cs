using System;
using MouseLab.Core.Models;

namespace MouseLabAvaloniaDesktop.Services.Abstractions;

public interface IMoveFinder
{
    Vector Find(IMatrix a, IMatrix b, int patchSize, int searchRange, bool fillZero = false);
}
