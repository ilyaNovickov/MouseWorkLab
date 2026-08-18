using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model.Core;
using MouseLabAvalonia.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MouseLabAvalonia
{
    /// <summary>
    /// Given a view model, returns the corresponding view if possible.
    /// Dockable content is resolved through its <see cref="IDockable.Context"/>.
    /// </summary>
    [RequiresUnreferencedCode(
        "Default implementation of ViewLocator involves reflection which may be trimmed away.",
        Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            var content = data is IDockable dockable ? (dockable.Context ?? dockable) : data;
            var type = ResolveViewType(content);

            if (type is null)
            {
                return new TextBlock { Text = "Not Found: " + content.GetType().FullName };
            }

            var control = (Control)Activator.CreateInstance(type)!;

            if (!ReferenceEquals(content, data))
            {
                control.DataContext = content;
            }

            return control;
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase
                || (data is IDockable dockable && ResolveViewType(dockable.Context ?? dockable) is not null);
        }

        private static Type? ResolveViewType(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            return type is not null && typeof(Control).IsAssignableFrom(type) ? type : null;
        }
    }
}
