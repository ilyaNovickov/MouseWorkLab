namespace MouseLab.Core
{
    public record struct Point
    {
        public static Point Zerp => new Point();

        public Point() : this(0, 0)
        {

        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { get; init; }

        public int Y { get; init;  }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator *(Point a, int x)
        {
            return new Point(a.X * x, a.Y * x);
        }

        public static Point operator /(Point a, int x)
        {
            return new Point(a.X / x, a.Y / x);
        }
    }
}
