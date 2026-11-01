using System;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public class WorktopViewModel : ViewModelBase
{
    private int? _seed = null;

    public WorktopViewModel(ITranslationService translation) : base(translation)
    {

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
