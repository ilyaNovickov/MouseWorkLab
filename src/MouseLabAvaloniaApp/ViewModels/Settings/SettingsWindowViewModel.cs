using MouseLabAvaloniaApp.Models;
using MouseLabAvaloniaApp.Services.AppSettings;
using ProTranslate;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace MouseLabAvaloniaApp.ViewModels.Settings
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        public SettingsWindowViewModel(
            ITranslationService translations,
            AppSettingsViewModel appSettingsViewModel
            ) : base(translations)
        {
            AppSettings = appSettingsViewModel;
        }

        public AppSettingsViewModel AppSettings { get; }

        // AppSettingsViewModel переживает окно: он подписан на CultureChanged
        // синглтона ICultureService, и без Dispose эта подписка держала бы его
        // живым до самого выхода из приложения. Двойное освобождение не страшно:
        // ViewModelBase.Dispose выставляет _disposed, второй вызов ничего не делает,
        // так что контейнер DI в Shutdown() может освободить его повторно.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                AppSettings.Dispose();

            base.Dispose(disposing);
        }
    }
}
