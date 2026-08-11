using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public class OptimizationReport
    {
        // Массивы для ScottPlot (он предпочитает double)
        public double[] P { get; init; }
        public double[] Difficulty { get; init; } // N
        public double[] Well { get; init; }       // G

        // Оптимальная точка для выделения на графике или вывода в текст
        public (double p, double s, double n, double g) Optimal { get; init; }

        public OptimizationReport(int count)
        {
            P = new double[count];
            Difficulty = new double[count];
            Well = new double[count];
        }
    }
}
