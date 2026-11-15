using System;
using CommunityToolkit.Mvvm.Input;
using MouseLab.Common.Models;
using MouseLab.Common.Services;
using MouseLab.Core.Models;
using MouseLab.Core.Services;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class WorktopCanvasViewModel : ViewModelBase
{
    private readonly ModelingEnv modelingEnv;

    public WorktopCanvasViewModel(ModelingEnv env)
    {
        this.modelingEnv = env;
        modelingEnv.ModelingSettingsChanged += OnModelingSettingsChanged;
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

    private void OnModelingSettingsChanged(object? sender, EventArgs e)
    {

    }

    [RelayCommand]
    private void Generate()
    {
        modelingEnv.GenerateWorktop();
    }
}
