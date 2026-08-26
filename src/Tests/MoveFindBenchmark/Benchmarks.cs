using BenchmarkDotNet.Attributes;
using Microsoft.VSDiagnostics;
using System;
using System.Security.Cryptography;
using MouseBaseLib;
using MouseStdLib;
using MouseStdLib.Providers;
using MouseUnsafeLib;
using System.Collections.Generic;
using MouseUnsafeLib.Finders;
using MouseBaseLib.Interfaces.Services;

namespace MoveFindBenchmark
{
    public class Scenario
    {
        public int Resolution { get; set; }

        public int PatchSize { get; set; }

        public int SearchRange => (Resolution - PatchSize) / 2;

        public override string ToString()
        {
            return $"R={Resolution}:s={SearchRange}:p={PatchSize}";
        }
    }

    // For more information on the VS BenchmarkDotNet Diagnosers see https://learn.microsoft.com/visualstudio/profiling/profiling-with-benchmark-dotnet
    [CPUUsageDiagnoser]
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
    public class Benchmarks
    {
        public static IEnumerable<Scenario> Scenarios => new[]
        {
            new Scenario { Resolution = 16, PatchSize =  4 },
            new Scenario { Resolution = 16, PatchSize =  8 },
            new Scenario { Resolution = 16, PatchSize = 12 },
            new Scenario { Resolution = 16, PatchSize =  4 },

            new Scenario { Resolution = 40, PatchSize = 10 },
            new Scenario { Resolution = 40, PatchSize = 20 },
            new Scenario { Resolution = 40, PatchSize = 30 },
            new Scenario { Resolution = 40, PatchSize = 6 },

            new Scenario { Resolution = 128, PatchSize = 32 },
            new Scenario { Resolution = 128, PatchSize = 64 },
            new Scenario { Resolution = 128, PatchSize = 96 },
            new Scenario { Resolution = 128, PatchSize = 20 },

            //new Scenario { Resolution = 512, PatchSize = 128 },
            //new Scenario { Resolution = 512, PatchSize = 256 },
            //new Scenario { Resolution = 512, PatchSize = 384 },
            //new Scenario { Resolution = 512, PatchSize = 205 },
        };

        [ParamsSource(nameof(Scenarios))]
        public Scenario Scenario { get; set; } = null!;

        IMatrixCutter cutter = new ImageCutter();

        IMatrix m1;
        IMatrix m2;

        IMatrix src;
        Point position;
        Vector vector;

        MoveFinderFast f1 = new();
        MoveFinderBoundary f2 = new();
        MoveFinderSimd f3 = new();
        MoveFinderSimdParallel f4 = new();
        MoveFinderSimdBoundary f5 = new();
        MoveFinderSimdBoundaryParallel f6 = new();

        [IterationSetup]
        public void Setup()
        {
            IMatrixRandomizer rnd = new ImageRandomizer();

            src = rnd.Randomize(Scenario.Resolution + 2 * Scenario.SearchRange, Scenario.Resolution + 2 * Scenario.SearchRange);

            position = new Point((src.Width - Scenario.Resolution) / 2, (src.Height  - Scenario.Resolution) / 2);

            Random random = new();

            vector = new Vector();
            vector.Dx = random.Next(-Scenario.SearchRange, +Scenario.SearchRange);
            vector.Dy = random.Next(-Scenario.SearchRange, +Scenario.SearchRange);

            m1 = cutter.Cut(src, position, new Size(Scenario.Resolution));
            m2 = cutter.Cut(src, position + vector, new Size(Scenario.Resolution));
        }

        [Benchmark]
        public Vector Fast()
        {
            return f1.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector Bounty()
        {
            return f2.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector Simd()
        {
            return f3.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector SimdParallel()
        {
            return f4.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector SimdBounty()
        {
            return f5.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector SimdBountyParallel()
        {
            return f6.Find(m1, m2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector Pooled()
        {
            using var p1 = cutter.Cut(src, position, new Size(Scenario.Resolution), PooledMatrixProvider.Instance);
            using var p2 = cutter.Cut(src, position + vector, new Size(Scenario.Resolution), PooledMatrixProvider.Instance);
            return f1.Find(p1, p2, Scenario.PatchSize, Scenario.Resolution);
        }

        [Benchmark]
        public Vector PooledDestination()
        {
            var d1 = (PooledImageMatrix)PooledMatrixProvider.Instance.Create(Scenario.Resolution, Scenario.Resolution);
            var d2 = (PooledImageMatrix)PooledMatrixProvider.Instance.Create(Scenario.Resolution, Scenario.Resolution);

            cutter.Cut(src, position, new Size(Scenario.Resolution), d1);
            cutter.Cut(src, position + vector, new Size(Scenario.Resolution), d2);

            Vector v = f1.Find(d1, d2, Scenario.PatchSize, Scenario.Resolution);

            d1.Dispose();
            d2.Dispose();
            return v;
        }
    }
}
