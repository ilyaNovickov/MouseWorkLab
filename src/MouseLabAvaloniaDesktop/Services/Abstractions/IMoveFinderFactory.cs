using System;

namespace MouseLabAvaloniaDesktop.Services.Abstractions;

public interface IMoveFinderFactory
{
    IMoveFinder CreateSimpleFinder();

    IMoveFinder CreateSimpleBoundaryFinder();

    IMoveFinder CreateParallelFinder();

    IMoveFinder CreateParallelBoundaryFinder();
}
