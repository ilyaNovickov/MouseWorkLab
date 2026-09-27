using CommunityToolkit.Mvvm.ComponentModel;
using ProTranslate;
using ProTranslate.Generated;
using System;

namespace MouseLabAvaloniaApp.ViewModels;

/// <summary>
/// Базовый класс для всех моделей представления приложения.
/// </summary>
/// <remarks>
/// Наследует <see cref="ObservableObject"/> из CommunityToolkit.Mvvm, что даёт
/// <c>SetProperty</c> и <c>OnPropertyChanged</c> для реализации INotifyPropertyChanged.
/// </remarks>
public abstract class ViewModelBase : ObservableObject, IDisposable
{
    private bool _disposed;

    protected ViewModelBase(ITranslationService translations)
    {
        Translations = translations;

        // ProTranslateStrings генерируется SourceGenerator'ом по файлам
        // Assets\Translations\Strings.*.json. Для каждого ключа каталога в нём появляется
        // свойство (Strings.AppTitle, Strings.SettingsLanguage, ...), которое само вызывает
        // PropertyChanged при смене культуры. Поэтому в XAML достаточно
        // {Binding Strings.SomeKey}, и текст обновится без ручного кода.
        Strings = new ProTranslateStrings(translations);
    }

    /// <summary>
    /// Типизированные строки для привязки в XAML. Обновляются автоматически при смене культуры.
    /// </summary>
    public ProTranslateStrings Strings { get; }

    /// <summary>
    /// Низкоуровневый сервис переводов. Нужен, когда текст вычисляется в коде
    /// (Format/Observe), а не берётся напрямую из <see cref="Strings"/>.
    /// </summary>
    public ITranslationService Translations { get; }

    // Аварийные выходы для ключей, которые известны только в рантайме (например, ключ
    // приходит извне или собирается динамически). Предпочтительнее генерируемые
    // свойства Strings или константы ProTranslateKeys - они проверяются на этапе сборки
    // анализаторами PTA001 (существующий ключ) и PTA002 (совпадение числа плейсхолдеров).
    // PTA004 предупреждает именно об этом: динамический ключ проверить невозможно.
#pragma warning disable PTA004
    protected string T(string key) => Translations[key].Value;

    protected string T(string key, params object?[] arguments) => Translations.Format(key, arguments);
#pragma warning restore PTA004

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Шаблон освобождения ресурсов. Производные классы переопределяют метод, чтобы
    /// отписаться от событий, и обязаны вызывать base.Dispose(disposing).
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // ProTranslateStrings подписан на ITranslationService.CultureChanged.
            // Без Dispose подписка осталась бы висеть и удерживать модель представления.
            Strings.Dispose();
        }

        _disposed = true;
    }
}
