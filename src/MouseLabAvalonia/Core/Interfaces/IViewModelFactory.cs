using MouseLabAvalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.Core.Interfaces
{
    /// <summary>
    /// Фабрика, предоставляющая доступ к общим сервисам и ViewModel приложения
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Локатор окон приложения
        /// </summary>
        public IWindowLocator WindowsLocator { get; }

        /// <summary>
        /// Сервис диалоговых окон работы с файлами
        /// </summary>
        public IFileDialogService FileDialogService { get; }

        /// <summary>
        /// Сервис отображения сообщений и диалогов подтверждения
        /// </summary>
        public IMessageDialogService MessageDialogService { get; }

        /// <summary>
        /// ViewModel для работы с графиками
        /// </summary>
        public PlotViewModel PlotViewModel { get; }
    }
}
