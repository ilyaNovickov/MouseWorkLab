using MouseLabAvalonia.Core;
using MouseLabAvalonia.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvalonia.ViewModels
{
    /// <summary>
    /// Фабрика, предоставляющая доступ к общим сервисам и ViewModel приложения
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
    {
        public ViewModelFactory()
        {
            this.WindowsLocator = new WindowLocator();
            this.MessageDialogService = new MessegeDialogService(WindowsLocator);
            this.FileDialogService = new FileDialogService(WindowsLocator);
            this.PlotViewModel = new PlotViewModel(MessageDialogService, FileDialogService);
        }

        /// <summary>
        /// Локатор окон приложения
        /// </summary>
        public IWindowLocator WindowsLocator { get; private set; }

        /// <summary>
        /// Сервис диалоговых окон работы с файлами
        /// </summary>
        public IFileDialogService FileDialogService { get; private set; }

        /// <summary>
        /// Сервис отображения сообщений и диалогов подтверждения
        /// </summary>
        public IMessageDialogService MessageDialogService { get; private set; }

        /// <summary>
        /// ViewModel для работы с графиками
        /// </summary>
        public PlotViewModel PlotViewModel { get; private set; }
    }
}
