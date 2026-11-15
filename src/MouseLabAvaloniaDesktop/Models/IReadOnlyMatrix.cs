using System;
using System.Collections.Immutable;

namespace MouseLabAvaloniaDesktop.Models;

public interface IReadOnlyMatrix : IDisposable
{
    Size Size { get; }

    int Width { get; }

    int Height { get; }

    ImmutableArray<byte> ImmutableBytes { get; }

    ReadOnlySpan<byte> ReadOnlySpanBytes { get; }
}
