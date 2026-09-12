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
    /// <summary>
    /// Сервис отображения сообщений и диалогов подтверждения
    /// </summary>
    public class MessegeDialogService : IMessageDialogService
    {
        private readonly IWindowLocator windowLocator;

        /// <summary>
        /// Создание экземпляра сервиса сообщений
        /// </summary>
        /// <param name="windowLocator">Локатор окон приложения</param>
        public MessegeDialogService(IWindowLocator windowLocator)
        {
            this.windowLocator = windowLocator;
        }
        /// <summary>
        /// Показывает окно сообщения с кнопкой «OK»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        public async Task ShowOkAsync(string message, string caption = "", Icon icon = Icon.None)
        {
            var owner = windowLocator.GetRequiredWindow();

            var box = MessageBoxManager
                .GetMessageBoxStandard(caption, message,
                ButtonEnum.Ok, icon);

            await box.ShowWindowDialogAsync(owner);
        }

        /// <summary>
        /// Показывает диалог с кнопками «OK» и «Отмена»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        /// <returns><c>true</c>, если пользователь нажал «OK»</returns>
        public async Task<bool> ShowOkAbortAsync(string message, string caption = "", Icon icon = Icon.None)
        {
            var owner = windowLocator.GetRequiredWindow();

            var box = MessageBoxManager
                .GetMessageBoxStandard(caption, message,
                ButtonEnum.OkAbort, icon);

            ButtonResult result = await box.ShowWindowDialogAsync(owner);

            return result is ButtonResult.Ok ? true : false;
        }

        /// <summary>
        /// Показывает диалог с кнопками «Да» и «Нет»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        /// <returns><c>true</c>, если пользователь нажал «Да»</returns>
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
