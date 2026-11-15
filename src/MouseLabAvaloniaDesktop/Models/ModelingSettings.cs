using System;
using System.Runtime.Intrinsics.X86;
using MouseLab.Core.Services;

namespace MouseLabAvaloniaDesktop.Models;

public class ModelingSettings
{
    public int WorktopResolution
    {
        get;
        set;
    } = 1024;

    public bool ExperimantalSettings { get; set; } = false;
}
