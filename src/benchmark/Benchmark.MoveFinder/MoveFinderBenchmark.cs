using BenchmarkDotNet.Attributes;
using System;
using System.Security.Cryptography;
using MouseLab.Core.Impl.UnsafeVer1;
using MouseLab.Core.Impl.UnsafeVer2;
using System.Collections.Generic;
using MouseLab.Core.Models;
using MouseLab.Core;
using MouseLab.Core.Services;


namespace Benchmark.MoveFinder;

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

[MemoryDiagnoser]
[ThreadingDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
public class MoveFinderBenchmark
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

    IMatrixCutter cutter = new MatrixCutter();

    IMatrix? m1;
    IMatrix? m2;

    IMatrix? src;

    Point position;
    Vector vector;

    MoveFinderSimd f3 = new();
    MoveFinderSimdParallel f4 = new();
    MoveFinderSimdBoundary f5 = new();
    MoveFinderSimdBoundaryParallel f6 = new();

    SimdSadMatrixMatcher f7 = new();

    [IterationSetup]
    public void Setup()
    {
        IMatrixRandomizer rnd = new MatrixRandomizer();

        src = rnd.Randomize(Scenario.Resolution + 2 * Scenario.SearchRange, Scenario.Resolution + 2 * Scenario.SearchRange);

        position = new Point((src.Width - Scenario.Resolution) / 2, (src.Height - Scenario.Resolution) / 2);

        Random random = new();

        vector = new Vector();
        vector.Dx = random.Next(-Scenario.SearchRange, +Scenario.SearchRange);
        vector.Dy = random.Next(-Scenario.SearchRange, +Scenario.SearchRange);

        m1 = cutter.Cut(src, position, new Size(Scenario.Resolution));
        m2 = cutter.Cut(src, position + vector, new Size(Scenario.Resolution));
    }

    [Benchmark]
    public Vector Simd()
    {
        return f3.Find(m1!, m2!, Scenario.PatchSize, Scenario.SearchRange);
    }

    [Benchmark]
    public Vector SimdParallel()
    {
        return f4.Find(m1!, m2!, Scenario.PatchSize, Scenario.SearchRange);
    }

    [Benchmark]
    public Vector SimdBounty()
    {
        return f5.Find(m1!, m2!, Scenario.PatchSize, Scenario.SearchRange);
    }

    [Benchmark]
    public Vector SimdBountyParallel()
    {
        return f6.Find(m1!, m2!, Scenario.PatchSize, Scenario.SearchRange);
    }

    [Benchmark]
    public Vector NewImpl()
    {
        return f7.Match(m1!, m2!, Scenario.SearchRange, Scenario.SearchRange);
    }

}
