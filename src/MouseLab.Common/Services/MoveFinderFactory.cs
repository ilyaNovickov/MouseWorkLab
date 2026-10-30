using System;
using MouseLab.Common.Services.MoveFinder.Parallels;
using MouseLab.Common.Services.MoveFinder.Scalars;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services;

public sealed class MoveFinderFactory : IMoveFinderFactory
{
    private static MoveFinderSimd simple;

    private static MoveFinderSimdBoundary simpleBoundary;

    private static MoveFinderSimdParallel parallel;

    private static MoveFinderSimdBoundaryParallel parallelBoundary;

    static MoveFinderFactory()
    {
        simple = new();
        simpleBoundary = new();
        parallel = new();
        parallelBoundary = new();
    }

    public IMoveFinder CreateParallelBoundaryFinder()
    {
        return parallelBoundary;
    }

    public IMoveFinder CreateParallelFinder()
    {
        return parallel;
    }

    public IMoveFinder CreateSimpleBoundaryFinder()
    {
        return simpleBoundary;
    }

    public IMoveFinder CreateSimpleFinder()
    {
        return simple;
    }
}
