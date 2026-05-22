using MouseStdLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MouseUnsafeLib
{
    public abstract class MoveFinderSimdBase : StdMoveFinder
    {
        // Эффективное вычисление SAD для одной строки
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static unsafe int CalculateSAD(byte* p1, byte* p2, int length)
        {
            int sum = 0;
            int i = 0;

            // Использование AVX2 (обрабатываем по 32 байта за раз)
            if (Avx2.IsSupported && length >= 32)
            {
                Vector256<ulong> vSum = Vector256<ulong>.Zero;
                for (; i <= length - 32; i += 32)
                {
                    Vector256<byte> v1 = Avx2.LoadVector256(p1 + i);
                    Vector256<byte> v2 = Avx2.LoadVector256(p2 + i);

                    // PSADBW вычисляет SAD для каждых 8 байт и возвращает 4 ulong значения
                    Vector256<ulong> sad = Avx2.SumAbsoluteDifferences(v1, v2).AsUInt64<ushort>();
                    vSum = Avx2.Add(vSum, sad);
                }

                // Складываем частичные суммы из вектора
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1) + vSum.GetElement(2) + vSum.GetElement(3));
            }
            // Использование SSE2 (обрабатываем по 16 байт)
            else if (Sse2.IsSupported && length >= 16)
            {
                Vector128<ulong> vSum = Vector128<ulong>.Zero;
                for (; i <= length - 16; i += 16)
                {
                    Vector128<byte> v1 = Sse2.LoadVector128(p1 + i);
                    Vector128<byte> v2 = Sse2.LoadVector128(p2 + i);
                    Vector128<ulong> sad = Sse2.SumAbsoluteDifferences(v1, v2).AsUInt64<ushort>();
                    vSum = Sse2.Add(vSum, sad);
                }
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1));
            }

            // Остаток (если длина не кратна 16/32 или SIMD не поддерживается)
            for (; i < length; i++)
            {
                int diff = p1[i] - p2[i];
                sum += (diff < 0) ? -diff : diff;
            }

            return sum;
        }
    }
}
