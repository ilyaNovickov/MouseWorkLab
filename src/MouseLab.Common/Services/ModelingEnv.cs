using System;
using MouseLab.Core;
using MouseLab.Core.Models;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services;

public class ModelingEnv : IModelingEnv
{
    private IMatrix _worktop;

    private IMatrixRandomizer _matrixRandomizer;

    public ModelingEnv(IMoveFinderManager moveFinderManager, IModelingSettings modelingSettings, IMatrixRandomizer matrixRandomizer)
    {
        FinderManager = moveFinderManager;
        ModelingSettings = modelingSettings;

        _worktop = new Matrix(modelingSettings.WorktopResolution, modelingSettings.WorktopResolution);
        this._matrixRandomizer = matrixRandomizer;
    }

    public IMoveFinderManager FinderManager
    {
        get; private set;
    }

    public int WorktopHeight
    {
        get => ModelingSettings.WorktopResolution;
    }

    public int WorktopWidth
    {
        get => ModelingSettings.WorktopResolution;
    }

    public Size WorktopSize
    {
        get => new Size(WorktopWidth, WorktopHeight);
    }

    public ReadOnlySpan<byte> Worktop
    {
        get => _worktop.ReadOnlySpanBytes;
    }

    private int? _seed = null;

    public int? Seed
    {
        get => _seed;
        set
        {
            if (_seed == value)
                return;
            _seed = value;
        }
    }

    public IModelingSettings ModelingSettings
    {
        get; private set;
    }

    public void GenerateWorktop()
    {
        _matrixRandomizer.Randomize(_worktop, Seed);
    }
}
