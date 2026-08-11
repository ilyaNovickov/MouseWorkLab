using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace MouseUnsafeLib.Providers
{
    internal unsafe struct ConstantProvider : IVectorProvider
    {
        private readonly Vector256<byte> _v256;
        private readonly Vector128<byte> _v128;
        private readonly byte _const;

        public ConstantProvider(byte constant)
        {
            _const = constant;
            _v256 = Vector256.Create(constant);
            _v128 = Vector128.Create(constant);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector256<byte> GetVector256(int offset) => _v256;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<byte> GetVector128(int offset) => _v128;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte GetByte(int offset) => _const;
    }
}

