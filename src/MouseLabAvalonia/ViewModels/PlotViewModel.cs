
using CommunityToolkit.Mvvm.ComponentModel;
using Mouse.Services;
using MouseBaseLib;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace MouseLabAvalonia.ViewModels
{
    public class ReportChangedEventArgs : EventArgs
    {
        public ReportChangedEventArgs(OptimizationReport? report)
        {
            Report = report;
        }

        public OptimizationReport? Report { get; private set; }
    }

    public partial class PlotViewModel : ObservableValidator
    {
        private OptimizationReport? Report
        {
            get;
            set
            {
                field = value;
                ReportChanged?.Invoke(this, new ReportChangedEventArgs(field));
                IsOldData = false;
            }
        }
        public int MaxResolution => 128;
        public int MinResolution => 0;

        [ObservableProperty]
        [Range(0, 128, ErrorMessage = "Значение должно быть от 0 до 128")]
        private int? resolution = 10;

        [ObservableProperty]
        private bool isOldData = false;

        partial void OnResolutionChanging(int? oldValue, int? newValue)
        {
            if (oldValue == newValue)
                return;

            IsOldData = true;
        }

        // Важный момент для работы CommunityToolkit.Mvvm:
        // Нужно переопределить логику записи, чтобы запускалась валидация
        partial void OnResolutionChanged(int? value)
        {
            ValidateProperty(value, nameof(Resolution));

            if (value < MinResolution || value is null)
                Resolution = MinResolution;
            else if (value > MaxResolution)
                Resolution = MaxResolution;
        }

        public event EventHandler<ReportChangedEventArgs>? ReportChanged;

        public OptimizationReport? GetOptimizationReport()
        {
            if (this.Resolution == 0 || !this.Resolution.HasValue)
            {
                //messega here
                return null;
            }

            Report = ParametrOptimization.Calculate(this.Resolution.Value);
            return Report;

        }

        public string? CreateReportString()
        {
            if (Report is null || IsOldData)
                return null;

            StringBuilder builder = new();
            builder.AppendLine("p;s;N;G");

            for (int i = 0; i < Report.P.Length; i++)
            {
                builder.AppendLine($"{Report.P[i]};{(Resolution - Report.P[i]) / 2};{Report.Difficulty[i]};{Report.Well[i]}");
            }
            builder.AppendLine("OptimalPoint");
            builder.AppendLine($"{Report.Optimal.p};{(Resolution - Report.Optimal.p) / 2};{Report.Optimal.n};{Report.Optimal.g}");

            return builder.ToString();
        }
    }
}
