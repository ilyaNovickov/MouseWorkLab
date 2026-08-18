using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using MouseBaseLib;
using MouseLabAvalonia.ViewModels;
using ScottPlot;
using ScottPlot.Plottables;

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

        await plotVM.SaveReportAsync();
    }
}