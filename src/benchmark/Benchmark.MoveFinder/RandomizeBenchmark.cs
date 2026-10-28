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

[MemoryDiagnoser]
[ThreadingDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
public class RandomizeBenchmark
{
    [Params(10, 20, 40, 64, 128, 512, 1024)]
    public int Resolution { get; set; }

    IMatrixRandomizer rndNew = new MatrixRandomizer();
    ImageRandomizer rndOld = new();

    [GlobalSetup]
    public void Setup()
    {
        dest = new Matrix(Resolution, Resolution);
    }


    [Benchmark]
    public IMatrix RndNew()
    {
        return rndNew.Randomize(Resolution, Resolution);
    }

    [Benchmark]
    public IMatrix RndOld()
    {
        return rndOld.Randomize(Resolution, Resolution);
    }

    IMatrix dest = null!;

    [Benchmark]
    public void RndNewInto()
    {
        rndNew.Randomize(dest);
    }

    [Benchmark]
    public void RndOldInto()
    {
        rndOld.Randomize(dest);
    }
}

/// <summary>
/// Сервис получения случайной матрицы
/// </summary>
public class ImageRandomizer
{
    public IMatrix Randomize(Size size, int? seed = null)
        => Randomize(size.Width, size.Height, seed);



    public IMatrix Randomize(int width, int height, int? seed = null)
    {

        IMatrix matrix = new Matrix(width, height);

        Random random = seed is not null ? new Random(seed.Value) : new Random();
        random.NextBytes(matrix.SpanBytes);

        return matrix;
    }

    public void Randomize(IMatrix dest, int? seed = null)
    {
        Random random = seed is not null ? new Random(seed.Value) : new Random();
        random.NextBytes(dest.SpanBytes);
    }
}
