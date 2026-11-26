using System;
using System.Collections.Generic;
using System.Text;

namespace MouseLabAvaloniaDesktop.Models
{
    public struct PointD
    {
        public PointD()
        {

        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}
