using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MouseBaseLib
{
    /// <summary>
    /// Структура точки
    /// </summary>
    public record struct Point
    {
        public static Point Zero => new Point(0, 0);

        public Point() : this(0, 0)
        {

        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Положение X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Положение Y
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Перемещение точки на позицию (X, Y) 
        /// </summary>
        /// <param name="x">Новая координата X</param>
        /// <param name="y">Новая координата Y</param>
        public void Move(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Смещение точки на Dx и Dy 
        /// </summary>
        /// <param name="dx">Смещение по оси X</param>
        /// <param name="dy">Смещение по оси Y</param>
        public void Shift(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        /// <summary>
        /// Смещение точки на Dx и Dy 
        /// </summary>
        /// <param name="vector">Вектор смещения</param>
        public void Shift(Vector vector)
        {
            this.Shift(vector.Dx, vector.Dy);
        }

        /// <summary>
        /// Неизменяемый метод смещения точки
        /// </summary>
        /// <param name="dx">Смещение по оси X</param>
        /// <param name="dy">Смещение по оси Y</param>
        /// <returns>Новая точка</returns>
        public Point ShiftImmutable(int dx, int dy)
        {
            return new Point(X + dx, Y + dy);
        }
        /// <summary>
        /// Неизменяемый метод смещения точки
        /// </summary>
        /// <param name="vector">Вектор смещения</param>
        /// <returns>Новая точка</returns>
        public Point ShiftImmutable(Vector vector)
        {
            return new Point(X + vector.Dx, Y + vector.Dy);
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
