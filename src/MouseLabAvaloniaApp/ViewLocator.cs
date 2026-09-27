using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MouseLabAvaloniaApp.ViewModels;

namespace MouseLabAvaloniaApp;

/// <summary>
/// Maps a view model to the view that should render it inside a
/// <see cref="Avalonia.Controls.ContentControl"/>.
/// </summary>
/// <remarks>
/// <para>
/// The mapping is an explicit type switch rather than the reflection-based
/// "ViewModel" -> "View" name lookup from the Avalonia template. Reflection over a
/// computed type name cannot be statically analysed, so the trimmer removes it and a
/// NativeAOT publish silently renders "Not Found" instead of the view.
/// </para>
/// <para>
/// <c>MainWindowViewModel</c> is deliberately not mapped: the main window is assigned
/// directly in <c>App.OnFrameworkInitializationCompleted</c>, so mapping it here would
/// only risk nesting the window inside itself. Add one case per secondary view.
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
