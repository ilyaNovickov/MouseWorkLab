using Avalonia.Controls;
using MouseLabAvalonia.Core.Interfaces;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace MouseLabAvalonia.Core
{
    public class MessegeDialogService : IMessageDialogService
    {
        private readonly IWindowLocator windowLocator;

        public MessegeDialogService(IWindowLocator windowLocator)
        {
            this.windowLocator = windowLocator;
        }
        public async Task ShowOkAsync(string message, string caption = "", Icon icon = Icon.None)
        {
            var owner = windowLocator.GetRequiredWindow();

            var box = MessageBoxManager
                .GetMessageBoxStandard(caption, message,
                ButtonEnum.Ok, icon);

            await box.ShowWindowDialogAsync(owner);
        }

        public async Task<bool> ShowOkAbortAsync(string message, string caption = "", Icon icon = Icon.None)
        {
            var owner = windowLocator.GetRequiredWindow();

            var box = MessageBoxManager
                .GetMessageBoxStandard(caption, message,
                ButtonEnum.OkAbort, icon);

            ButtonResult result = await box.ShowWindowDialogAsync(owner);

            return result is ButtonResult.Ok ? true : false;
        }

        public async Task<bool> ShowYesNoAsync(string message, string caption = "", Icon icon = Icon.None)
        {
            var owner = windowLocator.GetRequiredWindow();

            var box = MessageBoxManager
                .GetMessageBoxStandard(caption, message,
                ButtonEnum.YesNo, icon);

            ButtonResult result = await box.ShowWindowDialogAsync(owner);

            return result is ButtonResult.Yes ? true : false;
        }
    }
}
