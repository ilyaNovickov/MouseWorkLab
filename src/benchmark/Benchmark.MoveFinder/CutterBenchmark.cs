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

public class Scenario2
{
    public int SrcResolution { get; set; }

    public int Resolution { get; set; }

    public override string ToString()
    {
        return $"SRC={SrcResolution}|R={Resolution}";
    }
}

[MemoryDiagnoser]
[ThreadingDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
public class CutterBenchmark
{
    public static IEnumerable<Scenario2> Scenarios => new[]
    {
        new Scenario2() { SrcResolution = 10, Resolution = 2 },
        new Scenario2() { SrcResolution = 10, Resolution = 5 },
        new Scenario2() { SrcResolution = 10, Resolution = 15 },
        new Scenario2() { SrcResolution = 40, Resolution = 20 },
        new Scenario2() { SrcResolution = 40, Resolution = 35 },
        new Scenario2() { SrcResolution = 40, Resolution = 60 },
        new Scenario2() { SrcResolution = 128, Resolution = 30 },
        new Scenario2() { SrcResolution = 128, Resolution = 60 },
        new Scenario2() { SrcResolution = 128, Resolution = 96 },
        new Scenario2() { SrcResolution = 128, Resolution = 164 },
    };

    [ParamsSource(nameof(Scenarios))]
    public Scenario2 Scenario { get; set; } = null!;

    IMatrix? src;
    Point pos;

    IMatrixCutter cutter = new MatrixCutter();

    ImageCutter cutterOld = new ImageCutter();

    [IterationSetup]
    public void Setup()
    {
        IMatrixRandomizer rnd = new MatrixRandomizer();

        src = rnd.Randomize(Scenario.SrcResolution, Scenario.SrcResolution);
        int size = (src.Width - Scenario.Resolution) / 2;
        pos = new Point(size, size);
    }

    [Benchmark]
    public IMatrix CutNew()
    {
        return cutter.Cut(src!, pos, Scenario.Resolution, Scenario.Resolution);
    }

    [Benchmark]
    public IMatrix CutOld()
    {
        return cutterOld.Cut(src!, pos, Scenario.Resolution, Scenario.Resolution);
    }
}

/// <summary>
/// Сервис вырезания области из матриц
/// </summary>
public class ImageCutter
{

    public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
    {
        IMatrix result = new Matrix(width, height);

        CutInto(src, position.X, position.Y, width, height, fillZero,
            result.SpanBytes);

        return result;
    }


    private static void CutInto(IMatrix src, int px, int py, int w, int h, bool fillZero, Span<byte> dst)
    {
        byte fill = fillZero ? byte.MinValue : byte.MaxValue;

        ReadOnlySpan<byte> srcData = src.ReadOnlySpanBytes;
        int sw = src.Width;
        int sh = src.Height;

        // Построчно: для каждой строки копируем непрерывный сегмент исходника
        // (внутри строки пиксели идут подряд) и заполняем граничные отступы.
        for (int y = 0; y < h; y++)
        {
            int sy = py + y;
            int dstRow = y * w;

            // Вся строка вне исходника — заполняем значением.
            if (sy < 0 || sy >= sh)
            {
                dst.Slice(dstRow, w).Fill(fill);
                continue;
            }

            int left = Math.Max(0, -px);      // столбцы с sx < 0
            int right = Math.Min(w, sw - px); // столбцы с sx >= sw

            if (left > 0)
                dst.Slice(dstRow, left).Fill(fill);

            if (right < w)
                dst.Slice(dstRow + right, w - right).Fill(fill);

            int count = right - left;
            if (count > 0)
            {
                int sx = px + left;
                srcData.Slice(sy * sw + sx, count).CopyTo(dst.Slice(dstRow + left, count));
            }
        }
    }
}
