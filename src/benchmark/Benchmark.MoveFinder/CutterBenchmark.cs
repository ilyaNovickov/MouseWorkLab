using BenchmarkDotNet.Attributes;
using System;
using System.Security.Cryptography;
using MouseLab.Core.Impl.UnsafeVer1;
using MouseLab.Core.Impl.UnsafeVer2;
using System.Collections.Generic;
using MouseLab.Core.Models;
using MouseLab.Core.Providers;
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
        // Малые окна: окно в разы меньше исходника, много заполняемых строк
        // и граней — стоимость вызова и аллокаций сопоставима с полезной
        // работой.
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
        // Крупные: окно шире исходника — почти вся строка непрерывного
        // копирования без граней (или полностью вне исходника).
        new Scenario2() { SrcResolution = 512, Resolution = 384 },
        // Порог параллельности Ver2 — dest > 4_000_000 px:
        // 768, 1024/1024 и 1024/1400 — ниже порога (однопоточная ветка),
        // 2048² = 4.19M px — единственный сценарий параллельной ветки.
        new Scenario2() { SrcResolution = 1024, Resolution = 768 },
        new Scenario2() { SrcResolution = 1024, Resolution = 1024 },
        new Scenario2() { SrcResolution = 1024, Resolution = 1400 },
        new Scenario2() { SrcResolution = 2048, Resolution = 2048 },
    };

    [ParamsSource(nameof(Scenarios))]
    public Scenario2 Scenario { get; set; } = null!;

    IMatrix? src;
    Point pos;
    Size destSize;

    // Интерфейсное поле: реалистичный вызов через IMatrixCutter — здесь
    // попадаем в интерфейсные default-методы (IFM-диспетчеризация).
    IMatrixCutter cutter = new MatrixCutter();

    // Конкретный класс: без IFM-хопов; отделяет стоимость диспетчеризации
    // от стоимости самого алгоритма.
    MatrixCutter cutterDirect = new MatrixCutter();

    ImageCutter cutterOld = new ImageCutter();

    // Пул PhoenixProvider'а: для CutNewPooled.
    IMatrixProvider provider = new MatrixProvider();

    [IterationSetup]
    public void Setup()
    {
        IMatrixRandomizer rnd = new MatrixRandomizer();

        src = rnd.Randomize(Scenario.SrcResolution, Scenario.SrcResolution);
        int size = (src.Width - Scenario.Resolution) / 2;
        pos = new Point(size, size);
        destSize = new Size(Scenario.Resolution, Scenario.Resolution);
    }

    // CutOld — базовая линия: остальные показываются относительно неё (ratio).

    [Benchmark(Baseline = true)]
    public IMatrix CutOld()
    {
        return cutterOld.Cut(src!, pos, Scenario.Resolution, Scenario.Resolution);
    }

    [Benchmark]
    public IMatrix CutNew()
    {
        return cutter.Cut(src!, pos, Scenario.Resolution, Scenario.Resolution);
    }

    [Benchmark]
    public IMatrix CutNewDirect()
    {
        return cutterDirect.Cut(src!, pos, destSize);
    }

    [Benchmark]
    public void CutNewPooled()
    {
        // Полный цикл через пул провайдера: аренда, копирование, возврат.
        // В steady state Allocated должен показать ~0 B — это и есть
        // главный выигрыш Ver2, который старый путь не даёт.
        var dest = cutter.Cut(src!, pos, destSize, provider);
        dest.Dispose();
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
