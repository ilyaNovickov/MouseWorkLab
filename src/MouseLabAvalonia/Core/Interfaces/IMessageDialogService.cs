using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace MouseLabAvalonia.Core.Interfaces
{
    /// <summary>
    /// Сервис отображения сообщений и диалогов подтверждения
    /// </summary>
    public interface IMessageDialogService
    {
        /// <summary>
        /// Показывает окно сообщения с кнопкой «OK»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        Task ShowOkAsync(string message, string caption = "", Icon icon = Icon.None);

        /// <summary>
        /// Показывает диалог с кнопками «OK» и «Отмена»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        /// <returns><c>true</c>, если пользователь нажал «OK»</returns>
        Task<bool> ShowOkAbortAsync(string message, string caption = "", Icon icon = Icon.None);

        /// <summary>
        /// Показывает диалог с кнопками «Да» и «Нет»
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="icon">Иконка, отображаемая в окне</param>
        /// <returns><c>true</c>, если пользователь нажал «Да»</returns>
        Task<bool> ShowYesNoAsync(string message, string caption = "", Icon icon = Icon.None);
    }
}
