using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    /// <summary>
    /// Отчёт для графиков
    /// </summary>
    public class OptimizationReport
    {
        // Массивы для ScottPlot (он предпочитает double)
        /// <summary>
        /// Массив размеров шаблона P
        /// </summary>
        public double[] P { get; init; }
        /// <summary>
        /// Массив вычислительной сложности N
        /// </summary>
        public double[] Difficulty { get; init; } // N
        /// <summary>
        /// Массив выгоды G
        /// </summary>
        public double[] Well { get; init; }       // G

        // Оптимальная точка для выделения на графике или вывода в текст
        /// <summary>
        /// Оптимальная точка (с максимальной выгодой g)
        /// </summary>
        public (double p, double s, double n, double g) Optimal { get; set; }

        public OptimizationReport(int count)
        {
            P = new double[count];
            Difficulty = new double[count];
            Well = new double[count];
        }
    }
}
