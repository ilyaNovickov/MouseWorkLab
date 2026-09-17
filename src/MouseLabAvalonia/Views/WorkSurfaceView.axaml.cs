using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MouseLabAvalonia.ViewModels;
using System;
using System.Runtime.InteropServices;

namespace MouseLabAvalonia.Views;

public partial class WorkSurfaceView : UserControl
{
    public WorkSurfaceView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (this.DataContext is WorkSurfaceViewModel vm)
        {
            vm.SurfaceChanged += Vm_SurfaceChanged;
        }
        base.OnDataContextChanged(e);
    }

    private void Vm_SurfaceChanged(object? sender, EventArgs e)
    {
        if (sender is not WorkSurfaceViewModel vm)
            return;

        if (vm.Surface == null)
            return;

        var bitmap = new WriteableBitmap(
            new PixelSize(vm.Surface.Width, vm.Surface.Height),
            new Vector(96, 96), // DPI
            PixelFormats.Gray8, // формат (с альфа-каналом)
            AlphaFormat.Premul
        );

        var imageData = vm.Surface.RawData;

        using (var frameBuffer = bitmap.Lock())
        {
            // Копируем байты прямо в буфер
            Marshal.Copy(imageData, 0, frameBuffer.Address, imageData.Length);
        }


        // Теперь bitmap можно использовать
        this.image.Source = bitmap;
    }
}