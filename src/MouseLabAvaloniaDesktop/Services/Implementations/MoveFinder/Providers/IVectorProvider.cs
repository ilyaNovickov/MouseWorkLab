using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Text;

namespace MouseLabAvaloniaDesktop.Services.Implementations.MoveFinder.Providers
{
    internal interface IVectorProvider
    {
        Vector256<byte> GetVector256(int offset);
        Vector128<byte> GetVector128(int offset);
        byte GetByte(int offset);
    }
}
