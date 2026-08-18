using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MouseBaseLib;
using MouseLabAvalonia.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ScottPlot;
using ScottPlot.Plottables;
using System.IO;

namespace MouseLabAvalonia.Views;

public partial class PlotView : UserControl
{
    public PlotView()
    {
        InitializeComponent();

        difficultPlot.Plot.Title($"Кол-во операций при разных параметрах");
        difficultPlot.Plot.XLabel("Разрешение шаблона p");
        difficultPlot.Plot.YLabel("Выч. сложность (N)");
        difficultPlot.Plot.ShowLegend(Alignment.UpperRight);

        paramPlot.Plot.Title("Лучшее значение размера шаблона");
        paramPlot.Plot.XLabel("Разрешение шаблона p");
        paramPlot.Plot.YLabel("Выгода (G)");
        paramPlot.Plot.ShowLegend(Alignment.UpperRight);

        difficultPlot.Refresh();
        paramPlot.Refresh();
    }

    private async void GetDataButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not PlotViewModel plotVM)
            return;

        OptimizationReport? report = plotVM.GetOptimizationReport();

        if (report is null)
        {
            return;
        }

        // Очистка старых данных
        difficultPlot.Plot.Clear();
        paramPlot.Plot.Clear();

        // Добавление новых данных (передаем массивы напрямую)
        Scatter? scatter1 = difficultPlot.Plot.Add.Scatter(report.P, report.Difficulty);
        scatter1.LegendText = $"R = {plotVM.Resolution}";
        difficultPlot.Plot.Axes.AutoScale();

        Scatter? scatter2 = paramPlot.Plot.Add.Scatter(report.P, report.Well);
        scatter2.LegendText = $"R = {plotVM.Resolution}";
        paramPlot.Plot.Axes.AutoScale();

        // Полезный бонус: ставим маркер на оптимальную точку
        Marker? optimalMarker = paramPlot.Plot.Add.Marker(report.Optimal.p, report.Optimal.g);
        optimalMarker.Color = Colors.Red;
        optimalMarker.Size = 10;
        paramPlot.Plot.Add.VerticalLine(report.Optimal.p, 2, Colors.Red);

        // Обновляем виджеты
        difficultPlot.Refresh();
        paramPlot.Refresh();
    }

    private async void CopyButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not PlotViewModel plotVM)
            return;

        string? str = await plotVM.CreateReportString();

        if (str is null)
            return;

        IClipboard? clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(str);
    }

    private async void SaveAsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not PlotViewModel plotVM)
            return;

        string? str = await plotVM.CreateReportString();

        if (str is null)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions 
        { 
            Title = "Сохранить файл",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
            },
            DefaultExtension = "csv"
        });

        if (file != null)
        {
            // Открываем поток для записи
            await using var stream = await file.OpenWriteAsync();
            using var streamWriter = new StreamWriter(stream);

            // Записываем содержимое
            await streamWriter.WriteLineAsync(str);
        }
        else
        {

        }
    }
}