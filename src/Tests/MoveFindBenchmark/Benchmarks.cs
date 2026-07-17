using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;
using System;
using System.Security.Cryptography;
using MouseBaseLib;
using MouseStdLib;
using MouseUnsafeLib;

namespace MoveFindBenchmark
{
    // For more information on the VS BenchmarkDotNet Diagnosers see https://learn.microsoft.com/visualstudio/profiling/profiling-with-benchmark-dotnet
    [CPUUsageDiagnoser]
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class Benchmarks
    {
        //private int R = 40;
        //private int p = 16;
        //private int R = 128;
        //private int p = 32;
        private int R = 512;
        private int p = 128;
        private int s => (R - p) / 2;

        IMatrixRandomizer rnd = new ImageRandomizer();
        IMatrixCutter cutter = new ImageCutter();

        IMatrix m1;
        IMatrix m2;

        MoveFinderFast f1 = new();
        MoveFinderBoundy f2 = new();
        MoveFinderSimd f3 = new();
        MoveFinderSimdParallel f4 = new();
        MoveFinderSimdBounty f5 = new();
        MoveFinderSimdBountyParallel f6 = new();

        [IterationSetup]
        public void Setup()
        {
            IMatrix src = rnd.Randomize(R + 2 * s, R + 2 * s);

            Point position = new Point((src.Width - R) / 2, (src.Height  - R) / 2);

            Random random = new();

            Vector vector = new Vector();
            vector.Dx = random.Next(-s, +s);
            vector.Dy = random.Next(-s, +s);

            m1 = cutter.Cut(src, position, new Size(R));
            m2 = cutter.Cut(src, position + vector, new Size(R));
        }

        //[Benchmark]
        //public Vector Fast()
        //{
        //    return f1.Find(m1, m2, p, R);
        //}

        //[Benchmark]
        //public Vector Bounty()
        //{
        //    return f2.Find(m1, m2, p, R);
        //}

        [Benchmark]
        public Vector Simd()
        {
            return f3.Find(m1, m2, p, R);
        }

        [Benchmark]
        public Vector SimdParallel()
        {
            return f4.Find(m1, m2, p, R);
        }

        [Benchmark]
        public Vector SimdBountyl()
        {
            return f5.Find(m1, m2, p, R);
        }

        [Benchmark]
        public Vector SimdBountyParallel()
        {
            return f6.Find(m1, m2, p, R);
        }
    }
}
