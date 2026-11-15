using System;

namespace MouseLabAvaloniaDesktop.Services.Abstractions;

public interface IModelingSettings
{
    int WorktopResolution { get; set; }

    int MaxWorktopResolution { get; }

    int ExpMaxWorktopResolution { get; }

    bool ExperimentalSettings { get; set; }
}
