using System;
using System.Collections.Generic;
using System.Drawing;
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

        public void Shift(Vector vector)
        {
            this.Shift(vector.Dx, vector.Dy);
        }

        public Point ShiftImmutable(int dx, int dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point ShiftImmutable(Vector vector)
        {
            return new Point(X + vector.Dx, Y + vector.Dy);
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

        public static Point operator +(Point point, Size size)
        {
            return point.ShiftImmutable(size.Width, size.Height);
        }

        public static Point operator +(Size size, Point point)
        {
            return point + size;
        }
    
        public static Point operator +(Point point, (int x, int y) turple)
        {
            return point.ShiftImmutable(turple.x, turple.y);
        }

        public static Point operator +((int x, int y) turple, Point point)
        {
            return point + turple;
        }

        public static Point operator +(Point point, Vector vector)
        {
            return point.ShiftImmutable(vector.Dx, vector.Dy);
        }

        public static Point operator +(Vector vector, Point point)
        {
            return point + vector;
        }

        public static Point operator -(Point point, Size size)
        {
            return point.ShiftImmutable(-size.Width, -size.Height);
        }

        public static Point operator -(Size size, Point point)
        {
            return point - size;
        }

        public static Point operator -(Point point, (int x, int y) turple)
        {
            return point.ShiftImmutable(-turple.x, -turple.y);
        }

        public static Point operator -((int x, int y) turple, Point point)
        {
            return point - turple;
        }

        public static Point operator -(Point point, Vector vector)
        {
            return point.ShiftImmutable(-vector.Dx, -vector.Dy);
        }

        public static Point operator -(Vector vector, Point point)
        {
            return point - vector;
        }
    }
}
