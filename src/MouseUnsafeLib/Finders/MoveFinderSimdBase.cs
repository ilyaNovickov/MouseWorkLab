using MouseStdLib;
using MouseUnsafeLib.Providers;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace MouseUnsafeLib.Finders
{
    public abstract class MoveFinderSimdBase : StdMoveFinder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int CalculateSADInternal<TProvider>(byte* p1, int length, TProvider provider) 
            where TProvider : struct, IVectorProvider
        {
            int sum = 0;
            int i = 0;

            // Использование AVX2 (обрабатываем по 32 байта за раз)
            if (Avx2.IsSupported && length >= 32)
            {
                Vector256<ulong> vSum = Vector256<ulong>.Zero;
                for (; i <= length - 32; i += 32)
                {
                    Vector256<byte> v1 = Avx.LoadVector256(p1 + i);
                    Vector256<byte> v2 = provider.GetVector256(i); // JIT подставит либо Load, либо возврат константы
                    // PSADBW вычисляет SAD для каждых 8 байт и возвращает 4 ulong значения
                    vSum = Avx2.Add(vSum, Avx2.SumAbsoluteDifferences(v1, v2).AsUInt64<ushort>());
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
                    Vector128<byte> v2 = provider.GetVector128(i);
                    vSum = Sse2.Add(vSum, Sse2.SumAbsoluteDifferences(v1, v2).AsUInt64<ushort>());
                }
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1));
            }
            // Для ARM процессоров (нетестировалось!!!)
            else if (AdvSimd.IsSupported && length >= 16)
            {
                Vector128<ulong> vSum = Vector128<ulong>.Zero;
                for (; i <= length - 16; i += 16)
                {
                    Vector128<byte> v1 = AdvSimd.LoadVector128(p1 + i);
                    Vector128<byte> v2 = provider.GetVector128(i);

                    Vector128<byte> abd = AdvSimd.AbsoluteDifference(v1, v2);
                    Vector128<ushort> sum16 = AdvSimd.AddPairwiseWidening(abd);
                    Vector128<uint> sum32 = AdvSimd.AddPairwiseWidening(sum16);
                    Vector128<ulong> sum64 = AdvSimd.AddPairwiseWidening(sum32);

                    vSum = AdvSimd.Add(vSum, sum64);
                }
                sum += (int)(vSum.GetElement(0) + vSum.GetElement(1));
            }

            // Остаток (если длина не кратна 16/32 или SIMD не поддерживается)
            for (; i < length; i++)
            {
                int diff = p1[i] - provider.GetByte(i);
                sum += (diff < 0) ? -diff : diff;
            }
            

            return sum;
        }

        // Эффективное вычисление SAD для одной строки
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static unsafe int CalculateSAD(byte* p1, byte* p2, int length)
        {
            return CalculateSADInternal(p1, length, new PointerProvider(p2));
        }

        public static unsafe int CalculateSADWithConstant(byte* p1, byte constant, int length)
        {
            return CalculateSADInternal(p1, length, new ConstantProvider(constant));
        }
    }
}
