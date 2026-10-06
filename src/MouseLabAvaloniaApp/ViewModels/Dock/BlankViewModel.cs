using System;
using ProTranslate;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MouseLabAvaloniaApp.ViewModels.Dock;

public partial class BlankViewModel : ViewModelBase
{
    public BlankViewModel(ITranslationService translation) : base(translation)
    {
        
    }

    public string Name { get; }  = "FOO";

    public string? MyValue
    {
        get;
        set
        {
            SetProperty(ref field, value);
        }
    } = "FOO";
}
