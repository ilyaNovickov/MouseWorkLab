using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;

namespace MouseLab.Core.Models
{
    public interface IMatrix : IDisposable
    {
        Size Size { get; }

        int Width { get; }

        int Height { get; }

        ImmutableArray<byte> ImmutableBytes { get; }

        byte[] Bytes { get; }

        Span<byte> SpanBytes { get; }

        ReadOnlySpan<byte> ReadOnlySpanBytes { get; }
    }
}
