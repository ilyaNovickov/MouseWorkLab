using System;
using System.Runtime.Intrinsics.X86;
using MouseLab.Core.Services;

namespace MouseLab.Common.Services;

public class ModelingSettings : IModelingSettings
{
    private int _worktopResolution = 8192;

    public int WorktopResolution
    {
        get => _worktopResolution;
        set
        {
            if (_worktopResolution == value)
                return;

            if (value < 0)
                throw new ArgumentException("Value can't be negative");

            if (!ExperimentalSettings && value > MaxWorktopResolution)
                throw new ArgumentException($"Value must be less than {MaxWorktopResolution} in standart settings");

            if (ExperimentalSettings && value > ExpMaxWorktopResolution)
                throw new ArgumentException($"Value must be less than {ExpMaxWorktopResolution} in experimental settings");

            _worktopResolution = value;
        }
    }

    public int MaxWorktopResolution => 8192;

    public int ExpMaxWorktopResolution => 16_384;

    public bool ExperimentalSettings
    {
        get;
        set
        {
            if (!value && WorktopResolution > MaxWorktopResolution)
            {
                WorktopResolution = MaxWorktopResolution;
            }
            field = value;
        }
    }
}
