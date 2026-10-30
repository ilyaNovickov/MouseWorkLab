using System;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services;

public sealed class MoveFinderManager : IMoveFinderManager
{
    private const int MinCoresForParallel = 4;
    private const double ParallelWorkThreshold = 44_100;

    private int _resolution = 12;
    private int _patchSize = 6;
    private int _searchRange = 3;
    private bool _relateSearchWithPatch = false;

    private IMoveFinder SimpleFinder { get; }
    private IMoveFinder SimpleBoundaryFinder { get; }
    private IMoveFinder ParallelFinder { get; }
    private IMoveFinder ParallelBoundaryFinder { get; }

    public MoveFinderManager(IMoveFinderFactory factory)
    {
        SimpleFinder = factory.CreateSimpleFinder();
        SimpleBoundaryFinder = factory.CreateSimpleBoundaryFinder();
        ParallelFinder = factory.CreateParallelFinder();
        ParallelBoundaryFinder = factory.CreateParallelBoundaryFinder();

        SelectedFinder = SimpleFinder;
    }

    public int Resolution
    {
        get => _resolution;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Resolution can't be less than 0");
            if (value == _resolution)
                return;

            _resolution = value;
            Normalize();
        }
    }

    public int PatchSize
    {
        get => _patchSize;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Patch size can't be less than 0");
            if (value > _resolution)
                throw new ArgumentOutOfRangeException(nameof(value), "Patch size can't be greater than resolution");
            if (value == _patchSize)
                return;

            _patchSize = value;
            Normalize();
        }
    }

    public bool RelateSearchWithPatch
    {
        get => _relateSearchWithPatch;
        set
        {
            if (value == _relateSearchWithPatch)
                return;

            _relateSearchWithPatch = value;
            Normalize();
        }
    }

    public int SearchRange
    {
        get => _searchRange;
        set
        {
            if (_relateSearchWithPatch)
                throw new InvalidOperationException("Search range is derived from resolution and patch size");
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Search range can't be less than 0");

            int maxRange = (_resolution + _patchSize) / 2;
            if (value > maxRange)
                throw new ArgumentOutOfRangeException(nameof(value), "Search range can't be greater than (resolution + patch size) / 2");
            if (value == _searchRange)
                return;

            _searchRange = value;
            Normalize();
        }
    }

    private void Normalize()
    {
        _patchSize = Math.Min(_patchSize, _resolution);

        int maxRange = (_resolution + _patchSize) / 2;
        _searchRange = _relateSearchWithPatch
            ? (_resolution - _patchSize) / 2
            : Math.Min(_searchRange, maxRange);

        UpdateFinder();
    }

    private void UpdateFinder()
    {
        bool useBoundary = SearchRange > (Resolution - PatchSize) / 2;
        double work = EstimateWork(PatchSize, SearchRange);
        bool useParallel = ShouldUseParallel(work, Environment.ProcessorCount, SearchRange);

        SelectedFinder = (useParallel, useBoundary) switch
        {
            (true, true) => ParallelBoundaryFinder,
            (true, false) => ParallelFinder,
            (false, true) => SimpleBoundaryFinder,
            _ => SimpleFinder
        };
    }

    // N = p^2 * (2s + 1)^2 — худшее число сравнений байтов.
    private static double EstimateWork(int patchSize, int searchRange)
    {
        double candidates = 2.0 * searchRange + 1.0;
        return (double)patchSize * patchSize * candidates * candidates;
    }

    private static bool ShouldUseParallel(double work, int cores, int searchRange)
    {
        if (cores < MinCoresForParallel)
            return false;

        // Parallel.For делит только dy: при searchRange = 0 потоков нет.
        int iterations = 2 * searchRange + 1;
        int usableCores = Math.Min(cores, iterations);

        return usableCores >= 2 && work >= ParallelWorkThreshold;
    }

    public IMoveFinder SelectedFinder
    {
        get;
        private set;
    }

}
