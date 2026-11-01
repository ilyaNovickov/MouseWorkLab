using System;

namespace MouseLab.Core.Services;

public interface IModelingEnv
{
    IMoveFinderManager FinderManager { get; }

    int WorktopHeight { get; }

    int WorktopWidth { get; }

    Size WorktopSize { get; }

    ReadOnlySpan<byte> Worktop { get; }

    int? Seed { get; set; }

    IModelingSettings ModelingSettings { get; }

    void GenerateWorktop();
}
