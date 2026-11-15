using System;
using System.Threading.Tasks;
using MouseLabAvaloniaDesktop.ViewModels;

namespace MouseLabAvaloniaDesktop.Services.Abstractions;

public interface IDialogService
{
    void Show<T>() where T : ViewModelBase;

    Task<bool> ShowDialogAsync<T>() where T : ViewModelBase;
}
