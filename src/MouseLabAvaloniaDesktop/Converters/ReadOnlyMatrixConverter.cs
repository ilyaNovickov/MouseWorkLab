using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MouseLabAvaloniaDesktop.Models;

namespace MouseLabAvaloniaDesktop.Converters;

public class ReadOnlyMatrixConverter : IValueConverter
{
    private static ReadOnlyMatrixConverter? _instance;

    public static ReadOnlyMatrixConverter Instance => _instance ??= new();

    // Кэш: один битмап на поток (в UI-потоке один)
    [ThreadStatic]
    private static WriteableBitmap? _cachedBitmap;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyMatrix matrix)
            return null;

        int width = matrix.Width;
        int height = matrix.Height;

        if (width <= 0 || height <= 0)
            return null;

        // Переиспользуем битмап, если размер совпадает
        if (_cachedBitmap is null
            || _cachedBitmap.PixelSize.Width != width
            || _cachedBitmap.PixelSize.Height != height)
        {
            _cachedBitmap?.Dispose();
            _cachedBitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Avalonia.Vector(96, 96),
                PixelFormats.Gray8,
                AlphaFormat.Opaque);
        }

        // Копируем байты из матрицы в буфер битмапа
        using var frameBuffer = _cachedBitmap.Lock();

        ReadOnlySpan<byte> sourceBytes = matrix.ReadOnlySpanBytes;
        int rowBytes = frameBuffer.RowBytes;   // stride (может быть > width из-за выравнивания!)

        unsafe
        {
            byte* destBase = (byte*)frameBuffer.Address;

            for (int y = 0; y < height; y++)
            {
                ReadOnlySpan<byte> srcRow = sourceBytes.Slice(y * width, width);
                Span<byte> dstRow = new(destBase + y * rowBytes, width);
                srcRow.CopyTo(dstRow);
            }
        }
        /*
        А может
        using (var frameBuffer = bitmap.Lock())
        {
            // Копируем байты прямо в буфер
            Marshal.Copy(imageData, 0, frameBuffer.Address, imageData.Length);
        }
        */

        return _cachedBitmap;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Односторонняя привязка");
    }
}
/* Использовать как
<Window xmlns:conv="using:MouseLabAvaloniaDesktop.Converters">
    <Window.Resources>
        <conv:MatrixToImageConverter x:Key="MatrixToImage" />
    </Window.Resources>

    <Image Source="{Binding Matrix,
                            Converter={StaticResource MatrixToImage},
                            Mode=OneWay}" />

</Window>
*/