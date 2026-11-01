using System;

namespace MouseLab.Core.Services;

public interface IModelingSettings
{
    int WorktopResolution { get; set; }

    int MaxWorktopResolution { get; }

    int ExpMaxWorktopResolution { get; }

    bool ExperimentalSettings { get; set; }
}
