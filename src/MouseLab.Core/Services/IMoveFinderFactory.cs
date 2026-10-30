using System;

namespace MouseLab.Core.Services;

public interface IMoveFinderFactory
{
    IMoveFinder CreateSimpleFinder();

    IMoveFinder CreateSimpleBoundaryFinder();

    IMoveFinder CreateParallelFinder();

    IMoveFinder CreateParallelBoundaryFinder();
}
