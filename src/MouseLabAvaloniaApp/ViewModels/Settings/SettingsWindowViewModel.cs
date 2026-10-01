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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                AppSettings.Dispose();

            base.Dispose(disposing);
        }
    }
}
