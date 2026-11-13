using System;
using System.Runtime.Intrinsics.X86;
using MouseLab.Core.Services;

namespace MouseLab.Core.Models;

public class ModelingSettings
{
    public int WorktopResolution
    {
        get;
        set;
    } = 1024;

    /*
    public static int MinWorktopResolution => 1024;

    public static int MaxWorktopResolution => 8_192;

    public static int ExpMaxWorktopResolution => 16_384;

    private int _worktopResolution = 8192;

    public int WorktopResolution
    {
        get => _worktopResolution;
        set
        {
            if (_worktopResolution == value)
                return;

            if (value < MinWorktopResolution)
                throw new ArgumentException("Value can't be negative or less than min value");

            if (!ExperimentalSettings && value > MaxWorktopResolution)
                throw new ArgumentException($"Value must be less than {MaxWorktopResolution} in standart settings");

            if (ExperimentalSettings && value > ExpMaxWorktopResolution)
                throw new ArgumentException($"Value must be less than {ExpMaxWorktopResolution} in experimental settings");

            _worktopResolution = value;
        }
    }

    public bool ExperimentalSettings
    {
        get;
        set
        {
            if (value)
            {
                field = value;
            }
            else
            {
                field = value;
                if (WorktopResolution > MaxWorktopResolution)
                {
                    WorktopResolution = MaxWorktopResolution;
                }
            }
        }
    }
    */
}
