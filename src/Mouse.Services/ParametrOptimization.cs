using MouseBaseLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mouse.Services
{
    public static class ParametrOptimization
    {
        public static OptimizationReport Calculate(int R)
        {
            if (R <= 0 || R > 200) throw new ArgumentException("R out of range");

            int count = R + 1;
            var report = new OptimizationReport(count);

            // Временные массивы для расчетов нормализации
            double[] nValues = new double[count];
            double[] bValues = new double[count];
            double[] sValues = new double[count];

            for (int p = 0; p <= R; p++)
            {
                double s = (R - p) / 2.0;
                double n = Math.Pow(p, 2) * Math.Pow(R - p + 1, 2);
                double b = p * s;

                report.P[p] = p;
                sValues[p] = s;
                nValues[p] = n;
                bValues[p] = b;
            }

            // Нормализация
            double nMin = nValues.Min();
            double nMax = nValues.Max();
            double bMin = bValues.Min();
            double bMax = bValues.Max();

            double nRange = Math.Abs(nMax - nMin) < 1e-9 ? 1 : nMax - nMin;
            double bRange = Math.Abs(bMax - bMin) < 1e-9 ? 1 : bMax - bMin;

            double maxG = double.MinValue;
            int bestIdx = 0;

            for (int i = 0; i < count; i++)
            {
                double nNorm = (nValues[i] - nMin) / nRange;
                double bNorm = (bValues[i] - bMin) / bRange;

                report.Difficulty[i] = nValues[i]; // На график N
                report.Well[i] = bNorm - nNorm;    // На график G

                if (report.Well[i] > maxG)
                {
                    maxG = report.Well[i];
                    bestIdx = i;
                }
            }

            // Сохраняем оптимальную точку
            report.Optimal = (report.P[bestIdx], sValues[bestIdx], nValues[bestIdx], report.Well[bestIdx]);

            return report;
        }
    }
}
