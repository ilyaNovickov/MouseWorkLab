using System;
using CommunityToolkit.Mvvm.Input;
using MouseLab.Common.Services;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public partial class WorktopViewModel : TranslatedViewModelBase
{
    private ModelingEnv _env;
    private int? _seed = null;

    public WorktopViewModel(ITranslationService translation, ModelingEnv env, WorktopCanvasViewModel worktopCanvasViewModel) : base(translation)
    {
        _env = env;
        _env.ModelingSettingsChanged += OnModelingSettingsChanged;
        this.WorktopCanvasViewModel = worktopCanvasViewModel;
    }

    private void OnModelingSettingsChanged(object? sender, EventArgs e)
    {

    }

    public WorktopCanvasViewModel WorktopCanvasViewModel
    {
        get;
        private set
        {
            SetProperty(ref field, value);
        }
    }

    public int? Seed
    {
        get => _seed;
        set
        {
            SetProperty(ref _seed, value);
        }
    }


}
