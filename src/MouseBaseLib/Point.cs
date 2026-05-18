using System;
using System.Collections.Generic;
using System.Text;

namespace MouseBaseLib
{
    public struct Point : ICloneable
    {

        public Point() : this(0, 0)
        {

        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }


        public int X { get; set; }

        public int Y { get; set; }

        public void Move(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void Shift(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        public object Clone()
        {
            return new Point(this.X, this.Y);
        }

        public string ToString(string arg)
        {
            switch (arg.ToLower())
            {
                case "vs" or "veryshort" or "very short":
                    return $"(x:{X}|y:{Y})";
                case "s" or "short":
                    return $"(x : {X}, y : {Y})";
                case "f" or "full":
                    return $"Point : (x : {X}, y : {Y})";
                default:
                    return this.ToString()!;
            }
        }
    }
}
