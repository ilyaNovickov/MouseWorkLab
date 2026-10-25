
using System.Diagnostics.CodeAnalysis;
namespace MouseLab.Core
{
    public struct Point
    {
        public static Point Zero => new Point();

        public Point() : this(0, 0)
        {

        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; init; }

        public int Y { get; init; }

        public bool Equals(Point point)
        {
            return this.X == point.X && this.Y == point.Y;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not Point point)
                throw new Exception("The compared object is not point");

            return this.Equals(point);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y);
        }

        public static Point operator *(Point a, int x)
        {
            return new Point(a.X * x, a.Y * x);
        }

        public static Point operator /(Point a, int x)
        {
            return new Point(a.X / x, a.Y / x);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.Equals(b);
        }
    }
}
