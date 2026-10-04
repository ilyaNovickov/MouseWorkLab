using MouseLabAvaloniaApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaApp.Services.AppSettings
{
    //временно хранит настройки без файла
    public class TemporaryAppSettingsService : IApplicationSettingsService
    {
        private string _currentCultureName;

        public TemporaryAppSettingsService()
        {
            AppSettingsSnapshot snapshot = new AppSettingsSnapshot();

            CurrentAppTheme = snapshot.Theme;
            _currentCultureName = AppSettingsStore.ResolveCulture(snapshot.Culture).Name;

        }

        public Themes CurrentAppTheme
        {
            get;
            set
            {
                // Игнорируем повторную установку того же значения, иначе событие
                // сработает без причины.
                if (field == value)
                    return;

                field = value;
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(value));
            }
        }

        /// <summary>
        /// Код выбранной культуры ("ru-RU"). Значение нормализуется через
        /// <see cref="AppSettingsStore.ResolveCulture"/>, поэтому неизвестная или
        /// пустая строка превращается в культуру по умолчанию, а не в ошибку.
        /// </summary>
        public string CurrentCultureName
        {
            get => _currentCultureName;
            set
            {
                string resolved = AppSettingsStore.ResolveCulture(value).Name;
                if (_currentCultureName == resolved)
                    return;

                _currentCultureName = resolved;
                CultureChanged?.Invoke(this, new CultureChangedEventArgs(AppSettingsStore.ResolveCulture(resolved)));
            }
        }

        public event ThemeChangedEventHandler? ThemeChanged;

        public event CultureChangedEventHandler? CultureChanged;
    }
}
