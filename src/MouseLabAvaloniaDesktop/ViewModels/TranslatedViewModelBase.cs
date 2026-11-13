using System;
using ProTranslate;
using ProTranslate.Generated;

namespace MouseLabAvaloniaDesktop.ViewModels;

public class TranslatedViewModelBase : ViewModelBase, IDisposable
{
    private bool _disposed;

    protected TranslatedViewModelBase(ITranslationService translations)
    {
        Translations = translations;
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

    // Аварийные выходы для ключей, которые известны только в рантайме (например, ключ
    // приходит извне или собирается динамически). Предпочтительнее генерируемые
    // свойства Strings или константы ProTranslateKeys - они проверяются на этапе сборки
    // анализаторами PTA001 (существующий ключ) и PTA002 (совпадение числа плейсхолдеров).
    // PTA004 предупреждает именно об этом: динамический ключ проверить невозможно.
#pragma warning disable PTA004
    protected string T(string key) => Translations[key].Value;

    protected string T(string key, params object?[] arguments) => Translations.Format(key, arguments);
#pragma warning restore PTA004
}
