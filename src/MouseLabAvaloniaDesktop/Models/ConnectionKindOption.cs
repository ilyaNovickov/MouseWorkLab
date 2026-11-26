using System;
using MouseLabAvaloniaDesktop.ViewModels.Worktop.NodesAndMore;
using ProTranslate;

namespace MouseLabAvaloniaDesktop.Models;

public sealed class ConnectionKindOption : IDisposable
{
    public ConnectionKindOption(IObservableLocalizedString name, ViewModels.Worktop.NodesAndMore.ConnectionKinds kind)
    {
        DisplayName = name;
        Kind = kind;
    }

    public IObservableLocalizedString DisplayName { get; }

    public ViewModels.Worktop.NodesAndMore.ConnectionKinds Kind { get; }

    public override string ToString() => DisplayName.Value;

    public void Dispose()
    {
        (DisplayName as IDisposable)?.Dispose();
    }
}
