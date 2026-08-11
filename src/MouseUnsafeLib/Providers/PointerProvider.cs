using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MouseUnsafeLib.Providers
{
    internal unsafe struct PointerProvider : IVectorProvider
    {
        private readonly byte* _ptr;
        public PointerProvider(byte* ptr) => _ptr = ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector256<byte> GetVector256(int offset) => Avx.LoadVector256(_ptr + offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<byte> GetVector128(int offset) => Sse2.LoadVector128(_ptr + offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetByte(int offset) => _ptr[offset];
    }
}
