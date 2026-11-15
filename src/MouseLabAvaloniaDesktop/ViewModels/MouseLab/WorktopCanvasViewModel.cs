using System;
using MouseLab.Common.Models;
using MouseLab.Common.Services;
using MouseLab.Core.Models;
using MouseLab.Core.Services;

namespace MouseLabAvaloniaDesktop.ViewModels;

public class WorktopCanvasViewModel : ViewModelBase
{
    private readonly IModelingEnv modelingEnv;

    public WorktopCanvasViewModel(IModelingEnv env)
    {
        this.modelingEnv = env;
    }

    public IReadOnlyMatrix Worktop
    {
        get
        {
            return modelingEnv.Worktop;
        }
    }

    public int WorktopAvaibleResolution => modelingEnv.WorktopAvaibleResolution;

    public int WorktopResolution => modelingEnv.WorktopResolution;
}
