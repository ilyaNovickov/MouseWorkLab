using System;
using MouseLab.Core.Models;


namespace MouseLab.Core.Services;

public interface IModelingEnv
{
    IMoveFinderManager FinderManager { get; }

    int WorktopResolution { get; }

    int WorktopAvaibleResolution { get; }

    IReadOnlyMatrix Worktop { get; }

    int? Seed { get; set; }

    ModelingSettings ModelingSettings { get; }

    void GenerateWorktop();

    void ApplySettings(ModelingSettings settings);
}
