using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;

namespace MouseLabAvaloniaDesktop.Models
{
    public interface IMatrix : IReadOnlyMatrix
    {
        byte[] Bytes { get; }

        Span<byte> SpanBytes { get; }
    }
}
