using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;
using System;
using System.Security.Cryptography;
using MouseBaseLib;
using MouseStdLib;

namespace MoveFindBenchmark
{
    // For more information on the VS BenchmarkDotNet Diagnosers see https://learn.microsoft.com/visualstudio/profiling/profiling-with-benchmark-dotnet
    [CPUUsageDiagnoser]
    public class Benchmarks
    {
        //private int R = 40;
        //private int p = 16;
        private int R = 40;
        private int p = 34;
        private int s => (R - p) / 2;

        IMatrixRandomizer rnd = new ImageRandomizer();
        IMatrixCutter cutter = new ImageCutter();
        StdMoveFinder moveFinder = new();

        IMatrix m1;
        IMatrix m2;

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

        [Benchmark]
        public Vector Std()
        {
            return moveFinder.Find(m1, m2, p, R);
        }

        [Benchmark]
        public Vector Opt()
        {
            return moveFinder.OptFind(m1, m2, p, R);
        }

        [Benchmark]
        public Vector Fill()
        {
            return moveFinder.FillFind(m1, m2, p, R);
        }

        [Benchmark]
        public Vector Simd()
        {
            return moveFinder.FindSimd(m1, m2, p, R);
        }
    }
}
