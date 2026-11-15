using System;
using MouseLabAvaloniaDesktop.Models;


namespace MouseLabAvaloniaDesktop.Services.Abstractions;

public interface IMoveFinder
{
    Vector Find(IMatrix a, IMatrix b, int patchSize, int searchRange, bool fillZero = false);
}
