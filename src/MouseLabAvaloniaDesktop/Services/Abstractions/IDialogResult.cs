using System;

namespace MouseLabAvaloniaDesktop.ViewModels;

public interface IDialogResult
{
    Action<bool>? CloseAction { get; set; }
}
