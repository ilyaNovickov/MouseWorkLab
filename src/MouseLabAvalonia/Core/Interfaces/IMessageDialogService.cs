using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace MouseLabAvalonia.Core.Interfaces
{
    public interface IMessageDialogService
    {
        Task ShowOkAsync(string message, string caption = "", Icon icon = Icon.None);
        Task<bool> ShowOkAbortAsync(string message, string caption = "", Icon icon = Icon.None);
        Task<bool> ShowYesNoAsync(string message, string caption = "", Icon icon = Icon.None);
    }
}
