using System;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services;

public sealed class MoveFinderManager : IMoveFinderManager
{
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
        // cost = p^2 * (2s + 1)^2
        ulong cost = ((ulong)PatchSize * (ulong)PatchSize) * (2ul * (ulong)SearchRange + 1ul) * (2ul * (ulong)SearchRange + 1ul);

        // p = 10 | s = 10
        ulong thresholdCost = 44100ul;

        bool useParallel = Environment.ProcessorCount >= 4 && cost > thresholdCost;

        bool useBoundary = SearchRange > (Resolution - PatchSize) / 2;

        if (useParallel)
        {
            SelectedFinder = useBoundary ? ParallelBoundaryFinder : ParallelFinder;
        }
        else
        {
            SelectedFinder = useBoundary ? SimpleBoundaryFinder : SimpleFinder;
        }
    }

    public IMoveFinder SelectedFinder
    {
        get;
        private set;
    }

}
