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
using BenchmarkDotNet.Configs;

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
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.Method)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
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

    IMatrixCutter cutterOld = new ImageCutter();
    ImageCutter cutterOldDirect = new ImageCutter();

    // Пул PhoenixProvider'а: для CutNewPooled.
    IMatrixProvider provider = new MatrixProvider();

    [GlobalSetup]
    public void Setup()
    {
        IMatrixRandomizer rnd = new MatrixRandomizer();

        src = rnd.Randomize(Scenario.SrcResolution, Scenario.SrcResolution);
        int size = (src.Width - Scenario.Resolution) / 2;
        pos = new Point(size, size);
        destSize = new Size(Scenario.Resolution, Scenario.Resolution);
        dest = new Matrix(destSize.Width, destSize.Height);
    }

    // CutOld — базовая линия: остальные показываются относительно неё (ratio).

    [BenchmarkCategory("Alloc"), Benchmark(Baseline = true)]
    public IMatrix CutOld()
    {
        return cutterOld.Cut(src!, pos, destSize);
    }

    [BenchmarkCategory("Alloc"), Benchmark()]
    public IMatrix CutOldDirect()
    {
        return cutterOldDirect.Cut(src!, pos, destSize);
    }

    [BenchmarkCategory("Alloc"), Benchmark()]
    public IMatrix CutNew()
    {
        return cutter.Cut(src!, pos, destSize);
    }

    [BenchmarkCategory("Alloc"), Benchmark()]
    public void CutNewDirect()
    {
        cutterDirect.Cut(src!, pos, destSize);
    }

    IMatrix dest = null!;

    [BenchmarkCategory("Into"), Benchmark()]
    public void CutOldDest()
    {
        cutterOld.Cut(src!, pos, destSize, dest!);
    }

    [BenchmarkCategory("Into"), Benchmark()]
    public void CutNewDest()
    {
        cutter.Cut(src!, pos, destSize, dest!);
    }
    /*
    [Benchmark]
    public void CutNewPooled()
    {
        // Полный цикл через пул провайдера: аренда, копирование, возврат.
        // В steady state Allocated должен показать ~0 B — это и есть
        // главный выигрыш Ver2, который старый путь не даёт.
        var dest = cutter.Cut(src!, pos, destSize, provider);
        dest.Dispose();
    }
    */
}

/// <summary>
/// Изоляция узкого ядра вырезания на фиксированном случае 512×512 → 384×384
/// (позиция (64,64): окно целиком внутри — чистое копирование, 384 строки).
///
/// Замер полного пути показал (~1.8× на 512/384): CutNew/CutNewDirect/CutNewPooled
/// равны (~76 мкс), но старый ImageCutter — ~42 мкс. Это исключает пиннинг,
/// IFM-диспетчеризацию, указательные Span'ы и аллокацию. Класс разбирает
/// оставшиеся факторы в чистом виде, без IMatrix:
///   - CoreOldLoop:      тот же построчный цикл, что и в ImageCutter (без вызовов на строку);
///   - CoreRowCall:      та же строка, но отдельным статическим методом (один вызов на строку);
///   - CoreRowCallProps: как в реальном MatrixCutter: вызов на строку ПЛЮС
///                       interface-свойства (src.Width/Height, dest.Width/Height) читаются на КАЖДОЙ строке;
///   - CorePointerRow:   указатель и метод на строку, как в старой однопоточной ветке Ver2 до расщепления.
/// </summary>
[MemoryDiagnoser]
public class CoreCutterBenchmark
{
    const int CoreSrcSize = 512;
    const int CoreWindow = 384;

    static readonly int CorePos = (CoreSrcSize - CoreWindow) / 2; // 64 — окно целиком внутри

    byte[] coreSrc = null!;
    Matrix coreSrcMatrix = null!;

    // Свежие на каждую итерацию объекты-условия — как в полном пути:
    // BDN вызывает IterationSetup до каждого измерения (в тайминг не входит).
    IMatrix? freshSrc;
    Point freshPos;
    Size freshSize;
    readonly MatrixCutter cutterLocal = new();
    readonly ImageCutter cutterLocalOld = new();

    [GlobalSetup]
    public void Setup()
    {
        coreSrc = new byte[CoreSrcSize * CoreSrcSize];
        new Random(42).NextBytes(coreSrc);
        coreSrcMatrix = new Matrix(coreSrc, CoreSrcSize, CoreSrcSize);
    }

    [IterationSetup]
    public void FreshSetup()
    {
        // Точная копия условий полного пути: свежая LOH-матрица на итерацию,
        // случайное заполнение, окно целиком внутри (384², позиция 64,64).
        freshSrc = new MatrixRandomizer().Randomize(CoreSrcSize, CoreSrcSize, seed: 42);
        freshPos = new Point(CorePos, CorePos);
        freshSize = new Size(CoreWindow, CoreWindow);
    }

    // Полные пути в том же сеансе — воспроизводим 42-vs-76 локально:
    [Benchmark(Description = "FULL old ImageCutter, fresh src")]
    public IMatrix CoreOldPath()
    {
        return cutterLocalOld.Cut(freshSrc!, freshPos, CoreWindow, CoreWindow);
    }

    [Benchmark(Description = "FULL Ver2 direct, fresh src")]
    public IMatrix CoreVer2Direct()
    {
        return cutterLocal.Cut(freshSrc!, freshPos, freshSize);
    }

    // Внутри ядра, но на свежей как в полном пути src-матрице:
    [Benchmark(Description = "core: old inline row loop, fresh src")]
    public byte[] CoreOldLoopFresh()
    {
        byte[] dst = new byte[CoreWindow * CoreWindow];

        ReadOnlySpan<byte> srcData = freshSrc!.ReadOnlySpanBytes;
        int sw = CoreSrcSize, sh = CoreSrcSize, w = CoreWindow, h = CoreWindow;
        int px = CorePos, py = CorePos;
        byte fill = byte.MinValue;

        for (int y = 0; y < h; y++)
        {
            int sy = py + y;
            int dstRow = y * w;

            if (sy < 0 || sy >= sh)
            {
                dst.AsSpan(dstRow, w).Fill(fill);
                continue;
            }

            int left = Math.Max(0, -px);
            int right = Math.Min(w, sw - px);

            if (left > 0)
                dst.AsSpan(dstRow, left).Fill(fill);

            if (right < w)
                dst.AsSpan(dstRow + right, w - right).Fill(fill);

            int count = right - left;
            if (count > 0)
            {
                int sx = px + left;
                srcData.Slice(sy * sw + sx, count).CopyTo(dst.AsSpan(dstRow + left, count));
            }
        }

        return dst;
    }

    // Точная копия тела CutInto из ImageCutter: цикл с инлайн-кодом строки.
    [Benchmark(Description = "old inline row loop")]
    public byte[] CoreOldLoop()
    {
        byte[] dst = new byte[CoreWindow * CoreWindow];

        ReadOnlySpan<byte> srcData = coreSrc;
        int sw = CoreSrcSize, sh = CoreSrcSize, w = CoreWindow, h = CoreWindow;
        int px = CorePos, py = CorePos;
        byte fill = byte.MinValue;

        for (int y = 0; y < h; y++)
        {
            int sy = py + y;
            int dstRow = y * w;

            if (sy < 0 || sy >= sh)
            {
                dst.AsSpan(dstRow, w).Fill(fill);
                continue;
            }

            int left = Math.Max(0, -px);
            int right = Math.Min(w, sw - px);

            if (left > 0)
                dst.AsSpan(dstRow, left).Fill(fill);

            if (right < w)
                dst.AsSpan(dstRow + right, w - right).Fill(fill);

            int count = right - left;
            if (count > 0)
            {
                int sx = px + left;
                srcData.Slice(sy * sw + sx, count).CopyTo(dst.AsSpan(dstRow + left, count));
            }
        }

        return dst;
    }

    // Та же логика строки, но вынесенная в статический метод: один вызов на строку.
    [Benchmark(Description = "row-method call, direct arrays")]
    public byte[] CoreRowCall()
    {
        byte[] dst = new byte[CoreWindow * CoreWindow];
        Span<byte> dstData = dst;
        ReadOnlySpan<byte> srcData = coreSrc;

        for (int row = 0; row < CoreWindow; row++)
            CoreRowManaged(dstData, srcData, CoreSrcSize, CoreSrcSize, CoreWindow,
                CorePos, CorePos, row, 0);

        return dst;
    }

    [Benchmark(Description = "row call + interface props per row")]
    public byte[] CoreRowCallProps()
    {
        IMatrix dest = new Matrix(CoreWindow, CoreWindow);
        IMatrix src = coreSrcMatrix;
        byte fillValue = 0;

        ReadOnlySpan<byte> srcData = src.ReadOnlySpanBytes;
        Span<byte> dstData = dest.SpanBytes;

        for (int row = 0; row < dest.Height; row++)
            CoreRowManaged(dstData, srcData, src.Width, src.Height, dest.Width,
                CorePos, CorePos, row, fillValue);

        return dest.Bytes;
    }

    // Как старая однопоточная ветка Ver2 до расщепления: fixed + указатели,
    // метод на строку, однопоточный цикл.
    [Benchmark(Description = "pointer row-method call, single thread")]
    public unsafe byte[] CorePointerRow()
    {
        byte[] dst = new byte[CoreWindow * CoreWindow];

        fixed (byte* s = coreSrc, d = dst)
        {
            for (int row = 0; row < CoreWindow; row++)
                CoreRowPointer(s, d, CoreSrcSize, CoreSrcSize, CoreWindow, CorePos, CorePos, row, 0);
        }

        return dst;
    }

    private static void CoreRowManaged(Span<byte> dstBase, ReadOnlySpan<byte> srcBase,
        int srcWidth, int srcHeight, int destWidth, int px, int py, int row, byte fillValue)
    {
        var destRow = dstBase.Slice(row * destWidth, destWidth);
        int donorY = py + row;

        if ((uint)donorY >= (uint)srcHeight)
        {
            destRow.Fill(fillValue);
            return;
        }

        int from = Math.Max(0, -px);
        int to = Math.Min(srcWidth - px, destWidth);

        if (to <= from)
        {
            destRow.Fill(fillValue);
            return;
        }

        if (from > 0)
            destRow[..from].Fill(fillValue);

        srcBase.Slice(donorY * srcWidth + px + from, to - from)
            .CopyTo(destRow[from..to]);

        if (to < destWidth)
            destRow[to..].Fill(fillValue);
    }

    private static unsafe void CoreRowPointer(byte* srcBase, byte* dstBase, int srcWidth, int srcHeight,
        int destWidth, int px, int py, int row, byte fillValue)
    {
        var destRow = new Span<byte>(dstBase + (long)row * destWidth, destWidth);
        int donorY = py + row;

        if ((uint)donorY >= (uint)srcHeight)
        {
            destRow.Fill(fillValue);
            return;
        }

        int from = Math.Max(0, -px);
        int to = Math.Min(srcWidth - px, destWidth);

        if (to <= from)
        {
            destRow.Fill(fillValue);
            return;
        }

        if (from > 0)
            destRow[..from].Fill(fillValue);

        var srcRow = new ReadOnlySpan<byte>(srcBase + (long)donorY * srcWidth + px + from, to - from);
        srcRow.CopyTo(destRow[from..to]);

        if (to < destWidth)
            destRow[to..].Fill(fillValue);
    }
}

/// <summary>
/// Сервис вырезания области из матриц
/// </summary>
public sealed class ImageCutter : IMatrixCutter
{
    #region PublicInterfaceMethods
    public void Cut(IMatrix src, Point position, Size size, IMatrix dest, bool fillZero = true)
    {
        CutInto(src, position.X, position.Y, size.Width, size.Height, dest.SpanBytes, fillZero);
    }

    public IMatrix Cut(IMatrix src, Point position, Size size, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, size.Width, size.Height, fillZero);
    }

    public IMatrix Cut(IMatrix src, int x, int y, Size size, bool fillZero = true)
    {
        return Cut(src, x, y, size.Width, size.Height, fillZero);
    }

    public IMatrix Cut(IMatrix src, Point position, int width, int height, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, width, height, fillZero);
    }

    public IMatrix Cut(IMatrix src, int x, int y, int width, int height, bool fillZero = true)
    {
        var dest = new Matrix(width, height);
        CutInto(src, x, y, width, height, dest.SpanBytes, fillZero);
        return dest;
    }

    #region  WithProvider
    public IMatrix Cut(IMatrix src, Point position, Size size, IMatrixProvider provider, bool fillZero = true)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var dest = provider.Create(size.Width, size.Height);
        CutInto(src, position.X, position.Y, size.Width, size.Height, dest.SpanBytes, fillZero);
        return dest;
    }


    public IMatrix Cut(IMatrix src, int x, int y, Size size, IMatrixProvider provider, bool fillZero = true)
    {
        return Cut(src, x, y, size.Width, size.Height, provider, fillZero);
    }

    public IMatrix Cut(IMatrix src, Point position, int width, int height, IMatrixProvider provider, bool fillZero = true)
    {
        return Cut(src, position.X, position.Y, width, height, provider, fillZero);
    }

    public IMatrix Cut(IMatrix src, int x, int y, int width, int height, IMatrixProvider provider, bool fillZero = true)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var dest = provider.Create(width, height);
        CutInto(src, x, y, width, height, dest.SpanBytes, fillZero);

        return dest;
    }
    #endregion

    #endregion


    private static void CutInto(IMatrix src, int px, int py, int w, int h, Span<byte> dst, bool fillZero)
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
