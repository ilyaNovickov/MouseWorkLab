using System;
using CommunityToolkit.Mvvm.Input;
using ProTranslate;
using MouseLab.Common.Services;
using MouseLab.Core.Services;
using MouseLab.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MouseLabAvaloniaApp.ViewModels;

public partial class WorktopViewModel : ViewModelBase
{
    private int? _seed;
    private IModelingEnv _env;

    public WorktopViewModel(
            ITranslationService translation,
            IModelingEnv env
            ) : base(translation)
    {
        this._env = env;
        _seed = _env.Seed;
    }

    public int? Seed
    {
        get => _seed;
        set => SetProperty(ref _seed, value);
    }

    [RelayCommand]
    private void GenerateWorktop()
    {

        _env.Seed = Seed;
        _env.GenerateWorktop();
        OnPropertyChanged(nameof(Worktop));
    }

    public IReadOnlyMatrix Worktop
    {
        get
        {
            return _env.Worktop;
        }
    }

    public int WorktopResolution
    {
        get => _env.WorktopResolution;
    }
}
