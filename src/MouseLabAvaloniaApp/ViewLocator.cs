using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MouseLabAvaloniaApp.ViewModels;

namespace MouseLabAvaloniaApp;

/// <summary>
/// Сопоставляет модель представления с представлением, которое должно её отрисовать
/// внутри <c>ContentControl</c>.
/// </summary>
/// <remarks>
/// <para>
/// Соответствие задано явным switch по типу, а не рефлексивным поиском по имени
/// из шаблона Avalonia ("ViewModel" -&gt; "View"). Рефлексия по вычисленному имени
/// типа не анализируется статически, поэтому триммер её вырезает, и в NativeAOT-сборке
/// вместо представления молча отрисуется "Not Found".
/// </para>
/// <para>
/// <c>MainWindowViewModel</c> намеренно не сопоставлен: главное окно создаётся
/// напрямую в <c>App.OnFrameworkInitializationCompleted</c>, и сопоставление здесь
/// лишь рисковало бы вложить окно в само себя. Добавляйте по одному случаю на
/// каждое новое представление.
/// </para>
/// </remarks>
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        return new TextBlock { Text = "No view registered for " + param.GetType().FullName };
    }

    public bool Match(object? data) => data is ViewModelBase;
}
