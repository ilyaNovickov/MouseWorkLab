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

    private IMoveFinder SimpleFinder { get; }
    private IMoveFinder SimpleBoundaryFinder { get; }
    private IMoveFinder ParallelFinder { get; }
    private IMoveFinder ParallelBoundaryFinder { get; }

    private bool _isFinderUpdating = false;


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
            if (value == Resolution)
                return;
            if (value < 0)
                throw new ArgumentException("Resolution can't be less than 0");

            _resolution = value;
            OnResolutionUpdated();
        }
    }

    private void OnResolutionUpdated()
    {
        _isFinderUpdating = true;

        if (Resolution < PatchSize)
        {
            PatchSize = Resolution;
        }

        UpdateFinder();
    }

    public int PatchSize
    {
        get => _patchSize;
        set
        {
            if (value == PatchSize)
                return;
            if (value < 0)
                throw new ArgumentException("Patch size can't be less than 0");

            _patchSize = value > Resolution ? Resolution : value;
            OnPatchSizeUpdated();
        }
    }

    private void OnPatchSizeUpdated()
    {
        if (RelateSearchWithPatch)
        {
            SearchRange = (Resolution - PatchSize) / 2;
            goto UpdateFinder;
        }
        else
        {

            int threshold = (Resolution + PatchSize) / 2;
            if (SearchRange > threshold)
            {
                SearchRange = threshold;
                goto UpdateFinder;
            }
        }
    UpdateFinder:
        if (_isFinderUpdating)
            return;
        _isFinderUpdating = true;
        UpdateFinder();
    }

    public bool RelateSearchWithPatch
    {
        get; set;
    } = false;

    public int SearchRange
    {
        get => _searchRange;
        set
        {
            if (value == SearchRange)
                return;
            if (value < 0)
                throw new ArgumentException("Search range can't be less than 0");
            if (RelateSearchWithPatch)
            {
                _searchRange = (Resolution - PatchSize) / 2;
                return;
            }
            int threshold = (Resolution + PatchSize) / 2;
            _searchRange = value > threshold ? threshold : value;
            OnSearchRangeUpdate();
        }
    }

    private void OnSearchRangeUpdate()
    {
        if (_isFinderUpdating)
            return;
        _isFinderUpdating = true;
        UpdateFinder();
    }


    private ulong ParallelThreshold = Environment.ProcessorCount >= 4 ?
        1
        : ulong.MaxValue;

    private void UpdateFinder()
    {
        _isFinderUpdating = false;

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
