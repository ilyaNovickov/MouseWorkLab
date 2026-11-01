using System;
using MouseLab.Common.Models;
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

    public int WorktopResolution
    {
        get => ModelingSettings.WorktopResolution;
    }

    public int WorktopAvaibleResolution
    {
        get
        {
            int res = WorktopResolution + (int)(0.20d * WorktopResolution);
            return res;
        }
    }

    public IReadOnlyMatrix Worktop
    {
        get => new ReadOnlyMatrix(_worktop.Bytes, WorktopResolution, WorktopResolution);
    }

    public void GenerateWorktop()
    {
        if (_worktop.Width != WorktopResolution || _worktop.Height != WorktopResolution)
            _worktop = new Matrix(WorktopResolution, WorktopResolution);
        _matrixRandomizer.Randomize(_worktop, Seed);
    }
}
