using System;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.ViewModels;

public class WorktopViewModel : TranslatedViewModelBase
{
    private int? _seed = null;

    public WorktopViewModel(ITranslationService translation, WorktopCanvasViewModel worktopCanvasViewModel) : base(translation)
    {
        this.WorktopCanvasViewModel = worktopCanvasViewModel;
    }

    public WorktopCanvasViewModel WorktopCanvasViewModel { get; }

    public int? Seed
    {
        get => _seed;
        set
        {
            SetProperty(ref _seed, value);
        }
    }
}
